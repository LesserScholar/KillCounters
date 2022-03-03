using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core.ViewModelCollection;
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
}
