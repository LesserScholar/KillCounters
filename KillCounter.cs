
using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;


namespace KillCounters
{

    internal class KillCounterTypeDefiner : SaveableTypeDefiner
    {
        public KillCounterTypeDefiner() : base(852_768_580) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(KillCounter), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, KillCounter>));
        }
    }

    public class KillCounter
    {

        [SaveableField(1)]
        Dictionary<string, int> counters = new Dictionary<string, int>();

        [SaveableField(2)]
        int total = 0;

        public void AddKill(string id)
        {
            total++;
            if (counters.ContainsKey(id))
            {
                counters[id]++;
            }
            else
            {
                counters[id] = 1;
            }
        }

        public int Totals()
        {
            return total;
        }
        public int Bandits()
        {
            int result = 0;
            foreach (var c in counters)
            {
                CharacterObject characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(c.Key);
                if (characterObject != null && characterObject.Occupation == Occupation.Bandit)
                {
                    result += c.Value;
                }
            }
            return result;
        }
        public int Mercenaries()
        {
            int result = 0;
            foreach (var c in counters)
            {
                CharacterObject characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(c.Key);
                if (characterObject != null && characterObject.Occupation == Occupation.Mercenary)
                {
                    result += c.Value;
                }
            }
            return result;
        }
        public int Heroes()
        {
            int result = 0;
            foreach (var c in counters)
            {
                CharacterObject characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(c.Key);
                if (characterObject != null && characterObject.IsHero)
                {
                    result += c.Value;
                }
            }
            return result;
        }
        public int RegularTroops()
        {
            int result = 0;
            foreach (var c in counters)
            {
                CharacterObject characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(c.Key);
                if (characterObject == null || !characterObject.IsHero &&
                    characterObject.Occupation != Occupation.Mercenary && characterObject.Occupation != Occupation.Bandit)
                {
                    result += c.Value;
                }
            }
            return result;
        }
    }

    public class KillCountersBehavior : CampaignBehaviorBase
    {
        public static KillCountersBehavior Instance()
        {
            return Campaign.Current.GetCampaignBehavior<KillCountersBehavior>();
        }

        public static KillCounter Query(string StringId)
        {
            if (Instance()._counters.ContainsKey(StringId)) return Instance()._counters[StringId];
            return new KillCounter();
        }

        public KillCountersBehavior()
        {
            _counters = new Dictionary<string, KillCounter>();
        }

        public void AddKill(BasicCharacterObject strikerTroop, BasicCharacterObject attackedTroop)
        {
            if (strikerTroop == null || attackedTroop == null) return;
            var id = strikerTroop.StringId;
            if (!_counters.ContainsKey(id))
            {
                _counters[id] = new KillCounter();
            }
            _counters[id].AddKill(attackedTroop.StringId);
        }

        public override void RegisterEvents()
        {
        }

        //Dictionary<BasicCharacterObject, Tracker> _trackers;
        Dictionary<string, KillCounter> _counters;

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_counters", ref _counters);
        }
    }

    [HarmonyPatch(typeof(MapEventSide), nameof(MapEventSide.ApplySimulatedHitRewardToSelectedTroop))]
    class MapEventSidePatch : HarmonyPatch
    {
        static void Postfix(CharacterObject strikerTroop, CharacterObject attackedTroop, bool isFinishingStrike)
        {
            if (!isFinishingStrike || strikerTroop == null || attackedTroop == null) return;

            var counters = Campaign.Current.GetCampaignBehavior<KillCountersBehavior>();

            counters.AddKill(strikerTroop, attackedTroop);
        }
    }

    [HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
    class MissionPatch : HarmonyPatch
    {
        static void Postfix(Agent affectedAgent, Agent affectorAgent)
        {
            if (Campaign.Current != null)
            {
                var counters = Campaign.Current.GetCampaignBehavior<KillCountersBehavior>();
                if (counters != null && affectorAgent != null && affectedAgent != null)
                    counters.AddKill(affectorAgent.Character, affectedAgent.Character);
            }
        }
    }
}
