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
                    var xmlPath = Path.Combine(path1: assemblyLocation, path2: "StaminaActionStopConfig.xml");
                    StaminaActionStopConfig.InitializeXmlPath(xmlPath: xmlPath);
            
                    var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                    StaminaActionStopConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                    
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
                    if (dropdownMapping.TryGetValue(d, out int mappedValue))
                    {
                        StaminaActionStopConfig.staminaThresholdPhase.Value = mappedValue;
                    }
                };
            };
        }
    }
}