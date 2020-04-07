using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using SandBox;

namespace ArenaExpansion {
    public class MWAXArenaWeaponSwapLogic : MissionLogic {
        private Agent player = Agent.Main;
        private Settlement _settlement;
        public bool MWAXLoadoutSelect { get; set; }
        public bool MWAXWeaponsSwapped { get; private set; }
        public int MWAXLoadout { get; set; }

        public override void AfterStart() {
            base.AfterStart();

            _settlement = PlayerEncounter.LocationEncounter.Settlement;
            this.MWAXLoadoutSelect = false;
            this.MWAXWeaponsSwapped = false;
        }

        public override void OnMissionTick(float dt) {
            base.OnMissionTick(dt);

            if (!this.MWAXLoadoutSelect)
                return;

            if (this.MWAXWeaponsSwapped)
                return;

            if (this.player == null)
                this.player = Agent.Main;

            ArenaPracticeFightMissionController missionBehaviour = Mission.Current.GetMissionBehaviour<ArenaPracticeFightMissionController>();

            if (this.player != null && missionBehaviour.IsPlayerPracticing) {
                this.SwapEquipment(this.player, this.MWAXLoadout);
                this.MWAXWeaponsSwapped = true;
            }
        }

        private void SwapEquipment(Agent agent, int loadout) {
            CharacterObject characterObject = Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_1_" + this._settlement.MapFaction.Culture.StringId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_1_empire");

            for (int i = 0; i < 4; i++) {
                EquipmentElement equipmentFromSlot = characterObject.BattleEquipments.ToList<Equipment>()[loadout].GetEquipmentFromSlot((EquipmentIndex)i);
                agent.RemoveEquippedWeapon((EquipmentIndex)i);
                if (equipmentFromSlot.Item != null) {
                    MissionWeapon missionWeapon = new MissionWeapon(equipmentFromSlot.Item, agent.Origin?.Banner);
                    agent.EquipWeaponWithNewEntity((EquipmentIndex)i, ref missionWeapon);
                }
            }

            agent.WieldInitialWeapons();
        }
    }
}
