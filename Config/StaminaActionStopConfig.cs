using System.IO;
using BepInEx.Configuration;

namespace StaminaActionStop.Config;

internal static class StaminaActionStopConfig
{
    internal static ConfigEntry<bool> EnablePreActionCheck = null!;
    internal static ConfigEntry<bool> EnableStaminaCheck = null!;
    internal static ConfigEntry<bool> EnableThresholdValue = null!;
    internal static ConfigEntry<int> StaminaThresholdValue = null!;
    internal static ConfigEntry<bool> EnableThresholdPhase = null!;
    internal static ConfigEntry<int> StaminaThresholdPhase = null!;
    internal static ConfigEntry<bool> EnableHpThresholdValue = null!;
    internal static ConfigEntry<int> HpThresholdValue = null!;
    internal static ConfigEntry<bool> EnableManaThresholdValue = null!;
    internal static ConfigEntry<int> ManaThresholdValue = null!;

    internal static string XmlPath { get; private set; } = string.Empty;
    internal static string TranslationXlsxPath { get; private set; } = string.Empty;

    internal static void LoadConfig(ConfigFile config)
    {
            EnablePreActionCheck = config.Bind(
                section: ModInfo.Name,
                key: "Enable Pre-Action Check",
                defaultValue: false,
                description: 
                    "Stop actions before they start when stamina is low. Recommended for permadeath or hardcore play. May stop some actions that do not consume stamina.\n" +
                    "スタミナが低いと、アクション開始前に行動を停止します。ハードコアや永久死プレイに推奨。スタミナを消費しない行動も停止される場合があります。\n" +
                    "体力过低时在动作开始前停止行动。适用于硬核或永久死亡模式。某些不消耗体力的动作也可能会被停止。"
            );

            EnableStaminaCheck = config.Bind(
                section: ModInfo.Name,
                key: "Enable Stamina Check",
                defaultValue: true,
                description: "Stop actions when stamina drops during the action.\n" +
                             "行動中にスタミナが低下した場合、行動を停止します。\n" +
                             "在行动过程中体力下降到阈值以下时停止行动。"
            );

            EnableThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Enable Threshold Value",
                defaultValue: true,
                description: "Enable the stamina threshold value option.\n" +
                             "スタミナのしきい値を有効にします。\n" +
                             "启用体力阈值选项。"
            );
            
            StaminaThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold Value",
                defaultValue: 0,
                description: "The stamina threshold value at which actions will stop.\n" +
                             "スタミナの閾値。この値を下回るとアクションが停止します。\n" +
                             "体力阈值，低于此值时会停止当前动作。"
            );

            EnableThresholdPhase = config.Bind(
                section: ModInfo.Name,
                key: "Enable Threshold Phase",
                defaultValue: false,
                description: "Enable the stamina threshold phase option.\n" +
                             "スタミナフェーズのしきい値を有効にします。\n" +
                             "启用体力阶段阈值选项。"
            );
            
            StaminaThresholdPhase = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold Phase",
                defaultValue: 1,
                description: "The phase-based stamina threshold at which actions will stop. Set to 2 for 'Tired', 1 for 'Very Tired', or 0 for 'Exhausted'.\n" +
                             "アクションを停止するスタミナフェーズのしきい値です。2 は「疲労」、1 は「かなり疲労」、0 は「過労」を示し、このフェーズ以下でアクションを停止します。\n" +
                             "动作停止的体力阶段阈值。2 表示“疲劳”，1 表示“非常疲劳”，0 表示“过劳”；低于或等于该阶段时会停止当前动作。"
            );
            
            EnableHpThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Enable HP Threshold Value",
                defaultValue: false,
                description: "Enable the HP threshold value option.\n" +
                             "HPのしきい値を有効にします。\n" +
                             "启用生命值阈值选项。"
            );
            
            HpThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "HP Threshold Value",
                defaultValue: 1,
                description: "The HP threshold value at which actions will be stopped. Triggered when stamina drops below zero and begins damaging HP.\n" +
                             "HPの閾値。この値を下回るとスタミナがマイナスになった際にHPダメージが始まり、行動が停止します。\n" +
                             "生命值阈值。耐力降至负数开始扣除生命值时，当生命值低于该值将停止行动。"
            );
            
            EnableManaThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "Enable MP Threshold Value",
                defaultValue: false,
                description: "Enable the mana threshold value option.\n" +
                             "マナのしきい値を有効にします。\n" +
                             "启用法力阈值选项。"
            );

            ManaThresholdValue = config.Bind(
                section: ModInfo.Name,
                key: "MP Threshold Value",
                defaultValue: 1,
                description: "The mana threshold value at which actions will be stopped. Triggered when stamina and HP drop below zero and starts damaging mana. Useful when using the Mana Body feat.\n" +
                             "マナの閾値。この値を下回ると、スタミナとHPが0未満になり、疲労ダメージがマナに対して発生した際に行動が停止します。「マナの体」特性を使用している場合に有効です。\n" +
                             "法力值阈值。当耐力与生命值降至零以下并开始扣除法力值时，低于此值将停止当前动作。适用于启用「魔力之体」特性时。"
            );
        }
        
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
