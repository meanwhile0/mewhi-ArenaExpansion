using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using SandBox;

namespace ArenaExpansion {
    public class MWAXCore : MBSubModuleBase {

        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game) {
            base.OnGameInitializationFinished(game);

            //InformationManager.ShowInquiry(new InquiryData("MWAX", "Arena Expansion Active", true, false, "Okay", "", (Action)null, (Action)null, ""), false);
            InformationManager.DisplayMessage(new InformationMessage("[MWAX] Arena Expansion Active", Color.FromUint(14703633U)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            base.OnGameStart(game, gameStarterObject);

            if (!(game.GameType is Campaign))
                return;

            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            this.AddBehaviours(gameInitializer);
        }

        public override void OnCampaignStart(Game game, object starterObject) {
            base.OnCampaignStart(game, starterObject);
        }

        public override void OnGameLoaded(Game game, object initializerObject) {
            base.OnGameLoaded(game, initializerObject);
        }

        public override void OnMissionBehaviourInitialize(Mission mission) {
            base.OnMissionBehaviourInitialize(mission);
            this.AddMissionBehaviourTemp(mission);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer) {
            gameInitializer.AddBehavior(new MWAXArenaMaster());
        }

        private void AddMissionBehaviourTemp(Mission mission) {
            if (mission.HasMissionBehaviour<MWAXArenaWeaponSwapLogic>() || !mission.HasMissionBehaviour<ArenaPracticeFightMissionController>())
                return;

            mission.AddMissionBehaviour((MissionBehaviour)new MWAXArenaWeaponSwapLogic());
        }
    }
}
