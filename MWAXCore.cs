using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using SandBox;
using SandBox.Missions.MissionLogics.Arena;

namespace ArenaExpansion {
    public class MWAXCore : MBSubModuleBase {

        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game) {
            base.OnGameInitializationFinished(game);

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

        public override void OnMissionBehaviorInitialize(Mission mission) {
            base.OnMissionBehaviorInitialize(mission);

            this.AddMissionBehaviours(mission);
        }

        private void AddBehaviours(CampaignGameStarter gameInitializer) {
            gameInitializer.AddBehavior(new MWAXArenaMaster());
        }

        private void AddMissionBehaviours(Mission mission) {
            if (mission.HasMissionBehavior<MWAXArenaWeaponSwapLogic>() || !mission.HasMissionBehavior<ArenaPracticeFightMissionController>())
                return;

            mission.AddMissionBehavior((MissionBehavior)new MWAXArenaWeaponSwapLogic());
        }
    }
}
