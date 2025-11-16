using System.Collections.Generic;
using EvilMask.Elin.ModOptions;
using System.IO;
using System.Reflection;
using BepInEx;
using EvilMask.Elin.ModOptions.UI;
using StaminaActionStop.Config;
using UnityEngine;

namespace StaminaActionStop.UI
{
    public static class UIController
    {
        public static void RegisterUI()
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is BaseUnityPlugin plugin && plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
                    
                    var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
                    if (assemblyLocation != null)
                    {
                        var xmlPath = Path.Combine(path1: assemblyLocation, path2: "StaminaActionStopConfig.xml");
                        StaminaActionStopConfig.InitializeXmlPath(xmlPath: xmlPath);
                        
                        var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                        StaminaActionStopConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                    }

                    if (File.Exists(path: StaminaActionStopConfig.XmlPath))
                    {
                        using (StreamReader sr = new StreamReader(path: StaminaActionStopConfig.XmlPath))
                            controller.SetPreBuildWithXml(xml: sr.ReadToEnd());
                    }
                    
                    if (File.Exists(path: StaminaActionStopConfig.TranslationXlsxPath))
                    {
                        controller.SetTranslationsFromXslx(path: StaminaActionStopConfig.TranslationXlsxPath);
                    }
                    
                    RegisterEvents(controller: controller);
                }
            }
        }
        
        private static void RegisterEvents(ModOptionController controller)
        {
            controller.OnBuildUI += builder =>
            {
                var hlayout = builder.GetPreBuild<OptHLayout>(id: "hlayout01");
                hlayout.Base.childForceExpandHeight = false;
                
                var vlayout01 = builder.GetPreBuild<OptVLayout>(id: "vlayout01");
                vlayout01.Base.childForceExpandHeight = false;
                
                var vlayout02 = builder.GetPreBuild<OptVLayout>(id: "vlayout02");
                vlayout02.Base.childForceExpandHeight = false;
                
                var enablePreActionCheckToggle = builder.GetPreBuild<OptToggle>(id: "enablePreActionCheckToggle");
                enablePreActionCheckToggle.Checked = StaminaActionStopConfig.enablePreActionCheck.Value;
                enablePreActionCheckToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enablePreActionCheck.Value = isChecked;
                };
                
                var enableStaminaCheckToggle = builder.GetPreBuild<OptToggle>(id: "enableStaminaCheckToggle");
                enableStaminaCheckToggle.Checked = StaminaActionStopConfig.enableStaminaCheck.Value;
                enableStaminaCheckToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enableStaminaCheck.Value = isChecked;
                };
                
                var valueToggle = builder.GetPreBuild<OptToggle>(id: "valueToggle");
                valueToggle.Checked = StaminaActionStopConfig.enableThresholdValue.Value;
                valueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enableThresholdValue.Value = isChecked;
                };
                
                var slider = builder.GetPreBuild<OptSlider>(id: "slider01");
                slider.Title = StaminaActionStopConfig.staminaThresholdValue.Value.ToString();
                slider.Value = StaminaActionStopConfig.staminaThresholdValue.Value;
                slider.Step = 1;
                slider.OnValueChanged += v =>
                {
                    slider.Title = v.ToString();
                    StaminaActionStopConfig.staminaThresholdValue.Value = (int)v;
                };
                
                var dropdown = builder.GetPreBuild<OptDropdown>(id: "dropdown01");
                dropdown.OnValueChanged += d =>
                {
                    slider.Step = Mathf.Pow(f: 10, p: d);
                };
                
                var phaseToggle = builder.GetPreBuild<OptToggle>(id: "phaseToggle");
                phaseToggle.Checked = StaminaActionStopConfig.enableThresholdPhase.Value;
                phaseToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enableThresholdPhase.Value = isChecked;
                };
                
                var dropdownMapping = new Dictionary<int, int>
                {
                    { 0, 1 }, // Tired -> 1
                    { 1, 0 }  // Exhausted -> 0
                };
                
                var dropdown02 = builder.GetPreBuild<OptDropdown>(id: "dropdown02");
                dropdown02.OnValueChanged += d =>
                {
                    if (dropdownMapping.TryGetValue(key: d, value: out int mappedValue))
                    {
                        StaminaActionStopConfig.staminaThresholdPhase.Value = mappedValue;
                    }
                };
                
                var enableHpThresholdValueToggle = builder.GetPreBuild<OptToggle>(id: "enableHpThresholdValueToggle");
                enableHpThresholdValueToggle.Checked = StaminaActionStopConfig.enableHpThresholdValue.Value;
                enableHpThresholdValueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enableHpThresholdValue.Value = isChecked;
                };
                
                var slider02 = builder.GetPreBuild<OptSlider>(id: "slider02");
                slider02.Title = StaminaActionStopConfig.hpThresholdValue.Value.ToString();
                if (EClass.core.IsGameStarted == true)
                {
                    slider02.Max = (float)EClass.pc?.MaxHP;
                }
                slider02.Value = StaminaActionStopConfig.hpThresholdValue.Value;
                slider02.Step = 1;
                slider02.OnValueChanged += v =>
                {
                    slider02.Title = v.ToString();
                    StaminaActionStopConfig.hpThresholdValue.Value = (int)v;
                };
                
                var dropdown03 = builder.GetPreBuild<OptDropdown>(id: "dropdown03");
                dropdown03.OnValueChanged += d =>
                {
                    slider02.Step = Mathf.Pow(f: 10, p: d);
                };
                
                var enableMpThresholdValueToggle = builder.GetPreBuild<OptToggle>(id: "enableMpThresholdValueToggle");
                enableMpThresholdValueToggle.Checked = StaminaActionStopConfig.enableManaThresholdValue.Value;
                enableMpThresholdValueToggle.OnValueChanged += isChecked =>
                {
                    StaminaActionStopConfig.enableManaThresholdValue.Value = isChecked;
                };
                
                var slider03 = builder.GetPreBuild<OptSlider>(id: "slider03");
                slider03.Title = StaminaActionStopConfig.manaThresholdValue.Value.ToString();
                if (EClass.core.IsGameStarted == true)
                {
                    slider03.Max = (float)EClass.pc?.mana?.max;
                }
                slider03.Value = StaminaActionStopConfig.manaThresholdValue.Value;
                slider03.Step = 1;
                slider03.OnValueChanged += v =>
                {
                    slider03.Title = v.ToString();
                    StaminaActionStopConfig.manaThresholdValue.Value = (int)v;
                };
                
                var dropdown04 = builder.GetPreBuild<OptDropdown>(id: "dropdown04");
                dropdown04.OnValueChanged += d =>
                {
                    slider03.Step = Mathf.Pow(f: 10, p: d);
                };
            };
        }
    }
}