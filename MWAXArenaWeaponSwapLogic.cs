﻿using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using SandBox;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Encounters;
using SandBox.Missions.MissionLogics.Arena;
using TaleWorlds.CampaignSystem.Conversation;
using SandBox.Conversation;

namespace ArenaExpansion {
    public class MWAXArenaWeaponSwapLogic : MissionLogic {
        private Agent player = Agent.Main;
        private Settlement _settlement;
        public bool MWAXLoadoutSelect { get; set; }
        public bool MWAXWeaponsSwapped { get; set; }
        public bool MWAXEnteredFromMenu { get; set; }
        public int MWAXLoadout { get; set; }

        public override void AfterStart() {
            base.AfterStart();

            _settlement = PlayerEncounter.LocationEncounter.Settlement;
            this.MWAXLoadoutSelect = false;
            this.MWAXWeaponsSwapped = false;
        }

        public override void OnMissionTick(float dt) {
            base.OnMissionTick(dt);
            
            if (this.MWAXEnteredFromMenu) {
                ConversationMission.StartConversationWithAgent(this.Mission.Agents.FirstOrDefault<Agent>((Func<Agent, bool>)(x => x.Character != null && ((CharacterObject)x.Character).Occupation == Occupation.ArenaMaster)));
                this.MWAXEnteredFromMenu = false;
            }

            if (!this.MWAXLoadoutSelect)
                return;

            if (this.MWAXWeaponsSwapped)
                return;

            this.player = Agent.Main;

            ArenaPracticeFightMissionController missionBehaviour = Mission.Current.GetMissionBehavior<ArenaPracticeFightMissionController>();

            if (this.player != null && missionBehaviour.IsPlayerPracticing) {
                this.SwapEquipment(this.player, this.MWAXLoadout);
                this.MWAXWeaponsSwapped = true;
            }
        }

        private void SwapEquipment(Agent agent, int loadout) {
            MWAXConfig config = new MWAXConfig();
            CharacterObject characterObject = Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_" + config.MWAXWeaponStage() + "_" + this._settlement.MapFaction.Culture.StringId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_1_empire");

            for (int i = 0; i < 4; i++) {
                EquipmentElement equipmentFromSlot = characterObject.BattleEquipments.ToList<Equipment>()[loadout].GetEquipmentFromSlot((EquipmentIndex)i);
                agent.RemoveEquippedWeapon((EquipmentIndex)i);
                if (equipmentFromSlot.Item != null) {
                    MissionWeapon missionWeapon = new MissionWeapon(equipmentFromSlot.Item, equipmentFromSlot.ItemModifier, agent.Origin?.Banner);
                    agent.EquipWeaponWithNewEntity((EquipmentIndex)i, ref missionWeapon);
                }
            }

            agent.WieldInitialWeapons();
        }
    }
}
