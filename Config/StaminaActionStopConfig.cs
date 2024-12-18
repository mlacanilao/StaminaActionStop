using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;

namespace StaminaActionStop.Config
{
    /// <summary>
    /// Configuration handler for QuestPicker.
    /// Manages both BepInEx configuration and XML layout paths.
    /// </summary>
    internal static class StaminaActionStopConfig
    {
        internal static ConfigEntry<bool> enableThresholdValue;
        internal static ConfigEntry<int> staminaThresholdValue;
        internal static ConfigEntry<bool> enableThresholdPhase;
        internal static ConfigEntry<int> staminaThresholdPhase;
        
        /// <summary>
        /// Path to the XML layout configuration file.
        /// </summary>
        internal static string XmlPath { get; private set; }
        
        /// <summary>
        /// Path to the XLSX translations file.
        /// </summary>
        internal static string TranslationXlsxPath { get; private set; }
        
        internal static void LoadConfig(ConfigFile config)
        {
            enableThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Enable Threshold Value",
                defaultValue: true,
                description: "Enable the stamina threshold value option.\n" +
                             "スタミナのしきい値を有効にします。\n" +
                             "启用体力阈值选项。"
            );
            
            staminaThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold Value",
                defaultValue: 0,
                description: "The stamina threshold value at which actions will stop.\n" +
                             "スタミナの閾値。この値を下回るとアクションが停止します。\n" +
                             "体力阈值，低于此值时会停止当前动作。"
            );

            enableThresholdPhase = config.Bind(
                section: ModInfo.Name,
                key: "Enable Threshold Phase",
                defaultValue: false,
                description: "Enable the stamina threshold phase option.\n" +
                             "スタミナフェーズのしきい値を有効にします。\n" +
                             "启用体力阶段阈值选项。"
            );
            
            staminaThresholdPhase = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold Phase",
                defaultValue: 1,
                description: "The phase-based stamina threshold at which actions will stop. Set to 1 for 'Tired' phase or 0 for 'Exhausted' phase.\n" +
                             "スタミナフェーズのしきい値。1は「疲労」、0は「過労」を示し、このフェーズ以下ではアクションが停止します。\n" +
                             "体力阶段阈值。设置为 1 表示「疲劳」，设置为 0 表示「过劳」，低于此阶段时会停止当前动作。"
            );
        }

        /// <summary>
        /// Initializes the path to the XML layout configuration file.
        /// </summary>
        /// <param name="xmlPath">Path to the XML layout file.</param>
        internal static void InitializeXmlPath(string xmlPath)
        {
            if (File.Exists(path: xmlPath))
            {
                XmlPath = xmlPath;
            }
            else
            {
                XmlPath = string.Empty;
            }
        }
        
        internal static void InitializeTranslationXlsxPath(string xlsxPath)
        {
            if (File.Exists(path: xlsxPath))
            {
                TranslationXlsxPath = xlsxPath;
            }
            else
            {
                TranslationXlsxPath = string.Empty;
            }
        }
    }
}
