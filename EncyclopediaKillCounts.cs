using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Encyclopedia.Pages;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace KillCounters
{
    [PrefabExtension("EncyclopediaHeroPage", "descendant::Widget[@Id='InfoContainer']")]
    public sealed class HeroPagePatch : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionFileName(true)]
        public String File => "HeroPagePatch";

    }
    [PrefabExtension("EncyclopediaUnitPage", "descendant::BrushWidget/Children/Widget/Children/ListPanel/Children/Widget/Children")]
    public sealed class UnitPagePatch : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Child;
        public override int Index => 1;


        [PrefabExtensionFileName(true)]
        public String File => "UnitPagePatch";
    }


    [ViewModelMixin("RefreshValues", true)]
    public class ExtendEncyclopediaUnitPageVM : BaseViewModelMixin<EncyclopediaUnitPageVM>
    {
        [DataSourceProperty]
        public string KillsText { get; set; }
        [DataSourceProperty]
        public MBBindingList<StringPairItemVM> Kills { get; set; }

        public ExtendEncyclopediaUnitPageVM(EncyclopediaUnitPageVM vm) : base(vm)
        {
            KillsText = "";
            Kills = new MBBindingList<StringPairItemVM>();
        }
        public override void OnRefresh()
        {
            base.OnRefresh();

            KillsText = new TextObject("{=ltrSkEbRPv4}Kills").ToString();
            Kills.Clear();

            var vm = base.ViewModel;
            if (vm == null) return;
            var character = Traverse.Create(vm).Field("_character").GetValue<CharacterObject>();
            if (character == null) return;

            var counter = KillCountersBehavior.Query(character.StringId);

            Kills.Add(new StringPairItemVM(new TextObject("{=6nVL5daR87t}Regular Troops:").ToString(), counter.RegularTroops().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=5A34EzKTyLT}Heroes:").ToString(), counter.Heroes().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=0RhanD9YqlX}Bandits:").ToString(), counter.Bandits().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=Z4RgDFHmZUn}Mercenaries:").ToString(), counter.Mercenaries().ToString("N0")));
        }
    }


    [ViewModelMixin("RefreshValues", true)]
    public class ExtendEncyclopediaHeroPageVM : BaseViewModelMixin<EncyclopediaHeroPageVM>
    {
        [DataSourceProperty]
        public string KillsText { get; set; }
        [DataSourceProperty]
        public MBBindingList<StringPairItemVM> Kills { get; set; }

        public ExtendEncyclopediaHeroPageVM(EncyclopediaHeroPageVM vm) : base(vm)
        {
            KillsText = "";
            Kills = new MBBindingList<StringPairItemVM>();
        }
        public override void OnRefresh()
        {
            base.OnRefresh();

            KillsText = new TextObject("{=ltrSkEbRPv4}Kills").ToString();
            Kills.Clear();

            var vm = base.ViewModel;
            if (vm == null) return;
            var hero = Traverse.Create(vm).Field("_hero").GetValue<Hero>();
            if (hero == null) return;

            var counter = KillCountersBehavior.Query(hero.StringId);

            Kills.Add(new StringPairItemVM(new TextObject("{=6nVL5daR87t}Regular Troops:").ToString(), counter.RegularTroops().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=5A34EzKTyLT}Heroes:").ToString(), counter.Heroes().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=0RhanD9YqlX}Bandits:").ToString(), counter.Bandits().ToString("N0")));
            Kills.Add(new StringPairItemVM(new TextObject("{=Z4RgDFHmZUn}Mercenaries:").ToString(), counter.Mercenaries().ToString("N0")));
        }
    }

    [HarmonyPatch(typeof(DefaultEncyclopediaUnitPage), "InitializeSortControllers")]
    public class UnitPageSortingPatch {
        static void Postfix(ref IEnumerable<EncyclopediaSortController> __result)
        {
            var list = __result as List<EncyclopediaSortController>;
            if (list == null) return;
            list.Add(
                new EncyclopediaSortController(new TextObject("{=ltrSkEbRPv4}Kills", null), new UnitKillsComparer()));
        }
    }
    public class UnitKillsComparer : DefaultEncyclopediaUnitPage.EncyclopediaListUnitComparer
    {
        public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
        {
            return base.CompareUnits(x, y, _comparison);
        }

        public override string GetComparedValueText(EncyclopediaListItem item)
        {
            CharacterObject? characterObject = item.Object as CharacterObject;
            if (characterObject != null)
            {
                return KillCountersBehavior.Query(characterObject.StringId).Totals().ToString();
            }
            Debug.FailedAssert("Unable to get the kills of a non-character object. Error in KillCounters mod.");
            return "";
        }

        private static Func<CharacterObject, CharacterObject, int> _comparison = (CharacterObject c1, CharacterObject c2) =>
        {
            var k1 = KillCountersBehavior.Query(c1.StringId).Totals();
            var k2 = KillCountersBehavior.Query(c2.StringId).Totals();
            return k1.CompareTo(k2);
        };
    }


    [HarmonyPatch(typeof(DefaultEncyclopediaHeroPage), "InitializeSortControllers")]
    public class HeroPageSortingPatch
    {
        static void Postfix(ref IEnumerable<EncyclopediaSortController> __result)
        {
            var list = __result as List<EncyclopediaSortController>;
            if (list == null) return;
            list.Add(
                new EncyclopediaSortController(new TextObject("{=ltrSkEbRPv4}Kills", null), new HeroKillsComparer()));
        }
    }
    public class HeroKillsComparer : DefaultEncyclopediaHeroPage.EncyclopediaListHeroComparer
    {
        public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
        {
            return base.CompareHeroes(x, y, _comparison);
        }

        public override string GetComparedValueText(EncyclopediaListItem item)
        {
            Hero? hero = item.Object as Hero;
            if (hero != null)
            {
                return KillCountersBehavior.Query(hero.StringId).Totals().ToString();
            }
            Debug.FailedAssert("Unable to get the kills of a non-character object. Error in KillCounters mod.");
            return "";
        }

        private static Func<Hero, Hero, int> _comparison = (Hero c1, Hero c2) =>
        {
            var k1 = KillCountersBehavior.Query(c1.StringId).Totals();
            var k2 = KillCountersBehavior.Query(c2.StringId).Totals();
            return k1.CompareTo(k2);
        };
    }
}
