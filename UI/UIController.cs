using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;
using StaminaActionStop.Config;
using UnityEngine;

namespace StaminaActionStop.UI;

public static class UIController
{
    private static readonly Dictionary<int, int> PhaseDropdownMapping = new Dictionary<int, int>
    {
        { 0, StatsStamina.Tired },
        { 1, StatsStamina.VeryTired },
        { 2, StatsStamina.Exhausted }
    };

    private static readonly Dictionary<int, int> ReversePhaseDropdownMapping =
        PhaseDropdownMapping.ToDictionary(keySelector: kv => kv.Value, elementSelector: kv => kv.Key);

    public static void RegisterUI()
    {
        var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
        if (controller == null)
        {
            StaminaActionStop.LogError(message: "Failed to register Mod Options controller.");
            return;
        }

        var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var xmlPath = Path.Combine(path1: assemblyLocation, path2: "StaminaActionStopConfig.xml");
        var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");

        StaminaActionStopConfig.InitializeXmlPath(xmlPath: xmlPath);
        StaminaActionStopConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);

        if (File.Exists(path: StaminaActionStopConfig.XmlPath))
        {
            controller.SetPreBuildWithXml(xml: File.ReadAllText(path: StaminaActionStopConfig.XmlPath));
        }
        else
        {
            StaminaActionStop.LogError(message: $"Mod Options XML not found: {xmlPath}");
        }

        if (File.Exists(path: StaminaActionStopConfig.TranslationXlsxPath))
        {
            controller.SetTranslationsFromXslx(path: StaminaActionStopConfig.TranslationXlsxPath);
        }
        else
        {
            StaminaActionStop.LogError(message: $"Mod Options translations not found: {xlsxPath}");
        }

        RegisterEvents(controller: controller);
    }

    private static void RegisterEvents(ModOptionController controller)
    {
        controller.OnBuildUI += builder =>
        {
            var hlayout = builder.GetPreBuild<OptHLayout>(id: "hlayout01");
            if (hlayout != null)
            {
                hlayout.Base.childForceExpandHeight = false;
            }

            var vlayout01 = builder.GetPreBuild<OptVLayout>(id: "vlayout01");
            if (vlayout01 != null)
            {
                vlayout01.Base.childForceExpandHeight = false;
            }

            var vlayout02 = builder.GetPreBuild<OptVLayout>(id: "vlayout02");
            if (vlayout02 != null)
            {
                vlayout02.Base.childForceExpandHeight = false;
            }

            var enablePreActionCheckToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enablePreActionCheckToggle");
            if (enablePreActionCheckToggle != null)
            {
                enablePreActionCheckToggle.Checked = StaminaActionStopConfig.EnablePreActionCheck.Value;
                enablePreActionCheckToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnablePreActionCheck.Value = isChecked;
                };
            }

            var enableStaminaCheckToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableStaminaCheckToggle");
            if (enableStaminaCheckToggle != null)
            {
                enableStaminaCheckToggle.Checked = StaminaActionStopConfig.EnableStaminaCheck.Value;
                enableStaminaCheckToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnableStaminaCheck.Value = isChecked;
                };
            }

            var valueToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "valueToggle");
            if (valueToggle != null)
            {
                valueToggle.Checked = StaminaActionStopConfig.EnableThresholdValue.Value;
                valueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnableThresholdValue.Value = isChecked;
                };
            }

            var staminaThresholdSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "slider01");
            if (staminaThresholdSlider != null)
            {
                staminaThresholdSlider.Title = StaminaActionStopConfig.StaminaThresholdValue.Value.ToString();
                if (EClass.core.IsGameStarted == true &&
                    EClass.pc != null)
                {
                    staminaThresholdSlider.Max = EClass.pc._maxStamina;
                }

                staminaThresholdSlider.Value = StaminaActionStopConfig.StaminaThresholdValue.Value;
                staminaThresholdSlider.Step = 1;
                staminaThresholdSlider.OnValueChanged += value =>
                {
                    staminaThresholdSlider.Title = value.ToString();
                    StaminaActionStopConfig.StaminaThresholdValue.Value = (int)value;
                };
            }

            var staminaThresholdDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown01");
            if (staminaThresholdDropdown != null &&
                staminaThresholdSlider != null)
            {
                staminaThresholdDropdown.OnValueChanged += index =>
                {
                    staminaThresholdSlider.Step = Mathf.Pow(f: 10, p: index);
                };
            }

            var phaseToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "phaseToggle");
            if (phaseToggle != null)
            {
                phaseToggle.Checked = StaminaActionStopConfig.EnableThresholdPhase.Value;
                phaseToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnableThresholdPhase.Value = isChecked;
                };
            }

            var phaseDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown02");
            if (phaseDropdown != null)
            {
                if (ReversePhaseDropdownMapping.TryGetValue(
                        key: StaminaActionStopConfig.StaminaThresholdPhase.Value,
                        value: out int dropdownIndex))
                {
                    phaseDropdown.Value = dropdownIndex;
                }
                else
                {
                    phaseDropdown.Value = ReversePhaseDropdownMapping[key: StatsStamina.Tired];
                    StaminaActionStop.LogError(
                        message: $"Unsupported stamina threshold phase in config: {StaminaActionStopConfig.StaminaThresholdPhase.Value}. Defaulting UI to Tired.");
                }

                phaseDropdown.OnValueChanged += index =>
                {
                    if (PhaseDropdownMapping.TryGetValue(key: index, value: out int mappedValue))
                    {
                        StaminaActionStopConfig.StaminaThresholdPhase.Value = mappedValue;
                    }
                };
            }

            var enableHpThresholdValueToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableHpThresholdValueToggle");
            if (enableHpThresholdValueToggle != null)
            {
                enableHpThresholdValueToggle.Checked = StaminaActionStopConfig.EnableHpThresholdValue.Value;
                enableHpThresholdValueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnableHpThresholdValue.Value = isChecked;
                };
            }

            var hpThresholdSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "slider02");
            if (hpThresholdSlider != null)
            {
                hpThresholdSlider.Title = StaminaActionStopConfig.HpThresholdValue.Value.ToString();
                if (EClass.core.IsGameStarted == true &&
                    EClass.pc != null)
                {
                    hpThresholdSlider.Max = EClass.pc.MaxHP;
                }

                hpThresholdSlider.Value = StaminaActionStopConfig.HpThresholdValue.Value;
                hpThresholdSlider.Step = 1;
                hpThresholdSlider.OnValueChanged += value =>
                {
                    hpThresholdSlider.Title = value.ToString();
                    StaminaActionStopConfig.HpThresholdValue.Value = (int)value;
                };
            }

            var hpThresholdDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown03");
            if (hpThresholdDropdown != null &&
                hpThresholdSlider != null)
            {
                hpThresholdDropdown.OnValueChanged += index =>
                {
                    hpThresholdSlider.Step = Mathf.Pow(f: 10, p: index);
                };
            }

            var enableMpThresholdValueToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableMpThresholdValueToggle");
            if (enableMpThresholdValueToggle != null)
            {
                enableMpThresholdValueToggle.Checked = StaminaActionStopConfig.EnableManaThresholdValue.Value;
                enableMpThresholdValueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.EnableManaThresholdValue.Value = isChecked;
                };
            }

            var manaThresholdSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "slider03");
            if (manaThresholdSlider != null)
            {
                manaThresholdSlider.Title = StaminaActionStopConfig.ManaThresholdValue.Value.ToString();
                if (EClass.core.IsGameStarted == true &&
                    EClass.pc?.mana != null)
                {
                    manaThresholdSlider.Max = EClass.pc.mana.max;
                }

                manaThresholdSlider.Value = StaminaActionStopConfig.ManaThresholdValue.Value;
                manaThresholdSlider.Step = 1;
                manaThresholdSlider.OnValueChanged += value =>
                {
                    manaThresholdSlider.Title = value.ToString();
                    StaminaActionStopConfig.ManaThresholdValue.Value = (int)value;
                };
            }

            var manaThresholdDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown04");
            if (manaThresholdDropdown != null &&
                manaThresholdSlider != null)
            {
                manaThresholdDropdown.OnValueChanged += index =>
                {
                    manaThresholdSlider.Step = Mathf.Pow(f: 10, p: index);
                };
            }
        };
    }

    private static T? GetRequiredPreBuild<T>(OptionUIBuilder builder, string id) where T : OptUIElement
    {
        T? element = builder.GetPreBuild<T>(id: id);
        if (element == null)
        {
            StaminaActionStop.LogError(message: $"Missing Mod Options prebuilt element: {id}");
        }

        return element;
    }
}
