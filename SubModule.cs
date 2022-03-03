using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace KillCounters
{
    public class SubModule : MBSubModuleBase
    {
        Harmony _harmony = new Harmony("KillCounters");
        private UIExtender _extender = new UIExtender("KillCounters");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            _harmony.PatchAll();

            _extender.Register(typeof(SubModule).Assembly);
            _extender.Enable();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }

        override protected void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            var campaignStarter = starterObject as CampaignGameStarter;
            if (campaignStarter != null)
            {
                campaignStarter.AddBehavior(new KillCountersBehavior());
            }
        }
    }

}