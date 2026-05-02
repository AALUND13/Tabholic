using System;
using System.Collections.Generic;
using System.Linq;
using TabInfo.Utils;
using UIManager.Utils.Configs;
using UnboundLib;

namespace Tabholic.Utils {
    public static class AllowedCategories {
        private static List<string> AllowedCatgories = new List<string>() {
            "Basic Stats",
            "Advanced Character Stats",
            "Armor Stats",
            "CCM Stats",
            "null"
        };
        private static Dictionary<string, List<string>> AllowStats = new Dictionary<string, List<string>>() {
            {"AA Stats", new List<string>() {
                "Damage Per Seconds",
                "Bullets Per Seconds"
            }},
            {"Advanced Gun Stats", new List<string>() {
                "Bullet Gravity",
                "Bullet Size",
                "Bullet Speed",
                "Damage Growth",
                "Even Gun Spread",
                "Gun Spread"
            }},
            {"Useful Stats", new List<string>() {
                "Ghost Bullets",
                "Unblockable"
            }},
            {"JARL Stats", new List<string>() {
                "Total Armor"
            }}
        };

        public static bool IsAllowed(string category) {
            return AllowedCatgories.Any(allowedCategory => allowedCategory.Equals(category, StringComparison.OrdinalIgnoreCase))
                || AllowStats.Any(allowedStat => allowedStat.Key.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAllowed(string category, string stat) {
            return AllowedCatgories.Any(allowedCategory => allowedCategory.Equals(category, StringComparison.OrdinalIgnoreCase))
                || AllowStats.Any(allowedStat => allowedStat.Key.Equals(category, StringComparison.OrdinalIgnoreCase) && allowedStat.Value.Contains(stat));
        }

        public static void AddAllowCategory(string category) {
            AllowedCatgories.Add(category);
        }

        public static void AddAllowStats(string category, string stat) {
            if(AllowStats.TryGetValue(category, out var stats)) {
                stats.Add(stat);
            } else {
                AllowStats.Add(category, new List<string>() { stat });
            }
        }
    }

    public static class StatsAlias {
        private static Dictionary<string, Dictionary<string, string>> statsAlias = new Dictionary<string, Dictionary<string, string>>() {
            { "AA Stats", new Dictionary<string, string>() { { "Damage Per Seconds", "DPS" }, { "Bullets Per Seconds", "SPS" } } }
        };

        public static void AddAlias(string category, string statName, string statAlias) {
            if(statsAlias.TryGetValue(category, out var stats)) {
                stats.Add(statName, statAlias);
            } else {
                statsAlias.Add(category, new Dictionary<string, string>() { { statName, statAlias } });
            }
        }

        public static string GetStatAlias(string category, string statName) {
            if(statsAlias.TryGetValue(category, out var statAlias) && statAlias.ContainsKey(statName)) {
                return statAlias[statName];
            } else {
                return statName;
            }
        }
    }

    // Bases
    public interface IBaseStatCategory {
        bool CanDisplay(Player player);
        string Display();
        int Priority { get; }

        IBaseStat[] Stats { get; }
    }

    public interface IBaseStat {
        bool CanDisplay(Player player);
        string Display(Player player);
    }

    // Normal
    public class StatCategoryWrapper : IBaseStatCategory {
        public readonly StatCategory WrappingCategory;
        public readonly BoolGlobleConfig Enabled;

        private readonly IBaseStat[] stats;
        public IBaseStat[] Stats => stats;

        public int Priority => WrappingCategory.priority;

        public StatCategoryWrapper(StatCategory category) {
            WrappingCategory = category;
            Enabled = new BoolGlobleConfig($"enable_{category.name.ToLower()}", AllowedCategories.IsAllowed(category.name));

            stats = category.Stats.Select(stat => new StatWrapper(category, stat.Value)).OrderBy(s => s.WrappingStat.name).ToArray();
        }

        public bool CanDisplay(Player player) {
            return (Enabled.Value || !TabholicOptionMenu.SimplisticMode.Value) && (bool)WrappingCategory.InvokeMethod("DisplayCondition", player); ;
        }

        public string Display() {
            return WrappingCategory.name;
        }
    }

    public class StatWrapper : IBaseStat {
        public readonly Stat WrappingStat;
        public readonly StatCategory WrappingCategory;

        public readonly BoolGlobleConfig Enabled;

        public StatWrapper(StatCategory category, Stat stat) {
            WrappingCategory = category;
            WrappingStat = stat;
            Enabled = new BoolGlobleConfig($"enable_{category.name.ToLower()}_{stat.name.ToLower()}", AllowedCategories.IsAllowed(category.name, stat.name));
        }

        public bool CanDisplay(Player player) {
            return (Enabled.Value || !TabholicOptionMenu.SimplisticMode.Value) && (bool)WrappingStat.InvokeMethod("DisplayCondition", player);
        }

        public string Display(Player player) {
            return $"{StatsAlias.GetStatAlias(WrappingCategory.name, WrappingStat.name)}: {(string)WrappingStat.InvokeMethod("DisplayValue", player)}";
        }
    }

    // Preview
    public class PreviewStatCategoryWrapper : IBaseStatCategory {
        private readonly PreviewStatWrapper[] previewStatWrapper;

        public IBaseStat[] Stats => previewStatWrapper;

        public readonly string Name;
        public int Priority => -1;

        public PreviewStatCategoryWrapper(string name, PreviewStatWrapper[] stats) {
            Name = name;
            previewStatWrapper = stats.OrderBy(s => s.Name).ToArray();
        }

        public bool CanDisplay(Player player) {
            return true;
        }

        public string Display() {
            return Name;
        }
    }

    public class PreviewStatWrapper : IBaseStat {
        public readonly string Name;
        public readonly string Amount;

        public PreviewStatWrapper(string name, string amount) {
            Name = name;
            Amount = amount;
        }

        public bool CanDisplay(Player player) {
            return true;
        }

        public string Display(Player player) {
            return $"{Name}: {Amount}";
        }
    }
}
