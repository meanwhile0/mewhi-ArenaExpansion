using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Localization;
using SandBox;
using SandBox.Source.Towns;

namespace ArenaExpansion {
    public class MWAXArenaMaster : CampaignBehaviorBase {
        CampaignGameStarter campaignGame;
        List<CultureObject> visitedCultures = new List<CultureObject>();
        int previousLoadout = 0;
        private bool _enteredFromMenu = false;

        public override void RegisterEvents() {
            CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.MissionStarted.AddNonSerializedListener((object)this, new Action<IMission>(this.AfterMissionStarted));
        }

        public override void SyncData(IDataStore dataStore) {
            // nothing needs to be done
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter) {
            this.campaignGame = campaignGameStarter;
            this.AddPreDialogues(campaignGameStarter);
            this.AddGameMenus(campaignGameStarter);

            if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown) {
                this.AddLoadoutDialogues(this.campaignGame, Settlement.CurrentSettlement);
            }
        }

        public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero) {
            if (mobileParty != MobileParty.MainParty || !settlement.IsTown)
                return;
            this.AddLoadoutDialogues(this.campaignGame, settlement);
        }

        private void AfterMissionStarted(IMission mission) {
            if (_enteredFromMenu) {
                Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXEnteredFromMenu = true;
                this._enteredFromMenu = false;
            }
        }

        protected void AddGameMenus(CampaignGameStarter campaignGameStarter) {
            campaignGameStarter.AddGameMenuOption("town_arena", "mwax_mno_weapons_list", "{=mwax_arena_5}Choose weapon", new GameMenuOption.OnConditionDelegate(this.game_menu_enter_practice_fight_on_condition), new GameMenuOption.OnConsequenceDelegate(this.MWAX_game_menu_choose_weapon), false, 2, false);
        }

        protected void AddPreDialogues(CampaignGameStarter campaignGameStarter) {
            // Pre-weapons list
            campaignGameStarter.AddPlayerLine("mwax_arena_choose_weapon", "arena_master_enter_practice_fight_confirm", "mwax_arena_weapons_list", "{=mwax_arena_1}I'll choose my gear.", (ConversationSentence.OnConditionDelegate)null, (ConversationSentence.OnConsequenceDelegate)null, 200, (ConversationSentence.OnClickableConditionDelegate)null, (ConversationSentence.OnPersuasionOptionDelegate)null);
            campaignGameStarter.AddDialogLine("mwax_arena_choose_weapons_master_confirm", "mwax_arena_weapons_list", "mwax_arena_weapons_list", "{=mwax_arena_2}Alright, which weapon are you taking?", (ConversationSentence.OnConditionDelegate)null, (ConversationSentence.OnConsequenceDelegate)null, 100, (ConversationSentence.OnClickableConditionDelegate)null);

            // Post-match list
            campaignGameStarter.AddPlayerLine("mwax_arena_rematch_previous", "arena_master_post_practice_fight_talk", "close_window", "{=mwax_arena_3}Yeah. I'll take my previous loadout.", new ConversationSentence.OnConditionDelegate(this.MWAX_conversation_post_fight), new ConversationSentence.OnConsequenceDelegate(this.MWAX_conversation_join_arena_with_previous_loadout), 200, (ConversationSentence.OnClickableConditionDelegate)null, (ConversationSentence.OnPersuasionOptionDelegate)null);
            campaignGameStarter.AddPlayerLine("mwax_arena_rematch_new", "arena_master_post_practice_fight_talk", "mwax_arena_weapons_list", "{=mwax_arena_4}Sure. I'll take a new loadout.", new ConversationSentence.OnConditionDelegate(this.MWAX_conversation_post_fight), (ConversationSentence.OnConsequenceDelegate)null, 200, (ConversationSentence.OnClickableConditionDelegate)null, (ConversationSentence.OnPersuasionOptionDelegate)null);
        }

        protected void AddLoadoutDialogues(CampaignGameStarter campaignGameStarter, Settlement settlement) {
            // Populate weapons list
            if (!visitedCultures.Contains(settlement.MapFaction.Culture)) {
                MWAXConfig config = new MWAXConfig();
                CharacterObject characterObject = Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_" + config.MWAXWeaponStage() + "_" + settlement.MapFaction.Culture.StringId);

                for (int i = 0; i < characterObject.BattleEquipments.Count<Equipment>(); i++) {
                    string[] dialogueIdArr = new string[4];
                    string[] dialogueTextArr = new string[4];
                    string dialogueId = "mwax_arena_loadout_";
                    string dialogueText = "";
                    int loadout = i;

                    for (int x = 0; x < 4; x++) {
                        EquipmentElement equipmentFromSlot = characterObject.BattleEquipments.ToList<Equipment>()[i].GetEquipmentFromSlot((EquipmentIndex)x);
                        if (equipmentFromSlot.Item != null) {
                            dialogueIdArr[x] = equipmentFromSlot.Item.StringId;
                            dialogueTextArr[x] = equipmentFromSlot.Item.Name.ToString();
                        }
                    }
                    dialogueId = string.Join("_", dialogueIdArr.Where(s => !string.IsNullOrEmpty(s)));
                    dialogueText = string.Join(", ", dialogueTextArr.Where(s => !string.IsNullOrEmpty(s)));

                    campaignGameStarter.AddPlayerLine(dialogueId, "mwax_arena_weapons_list", "close_window", dialogueText, new ConversationSentence.OnConditionDelegate(() => this.MWAX_conversation_culture_match(settlement.MapFaction.Culture)), new ConversationSentence.OnConsequenceDelegate(() => this.MWAX_conversation_join_arena_with_selected_loadout(loadout)), 100, (ConversationSentence.OnClickableConditionDelegate)null, (ConversationSentence.OnPersuasionOptionDelegate)null);
                }

                visitedCultures.Add(settlement.MapFaction.Culture);

                // Return
                campaignGameStarter.AddPlayerLine("mwax_arena_return", "mwax_arena_weapons_list", "arena_master_enter_practice_fight", "{=mwax_arena_5}Actually, nevermind.", new ConversationSentence.OnConditionDelegate(() => this.MWAX_conversation_culture_match(settlement.MapFaction.Culture)), (ConversationSentence.OnConsequenceDelegate)null, 100, (ConversationSentence.OnClickableConditionDelegate)null, (ConversationSentence.OnPersuasionOptionDelegate)null);
            }
        }

        private static void OpenMissionWithSettingPreviousLocation(
      string previousLocationId,
      string missionLocationId) {
            Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId(missionLocationId);
            Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId(previousLocationId);
            PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, (Location)null, (CharacterObject)null, (string)null);
            Campaign.Current.GameMenuManager.NextLocation = (Location)null;
            Campaign.Current.GameMenuManager.PreviousLocation = (Location)null;
        }

        private bool MWAX_conversation_culture_match(CultureObject culture) {
            if (culture.Equals(Settlement.CurrentSettlement.Culture)) {
                return true;
            } else {
                return false;
            }
        }

        private bool MWAX_conversation_post_fight() {
            Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXLoadoutSelect = false;
            Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXWeaponsSwapped = false;
            return true;
        }

        public void MWAX_conversation_join_arena_with_previous_loadout() {
            this.MWAX_conversation_join_arena_with_selected_loadout(this.previousLoadout);
        }

        public void MWAX_conversation_join_arena_with_selected_loadout(int loadout) {
            this.previousLoadout = loadout;
            Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXLoadoutSelect = true;
            Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXLoadout = loadout;
            Campaign.Current.ConversationManager.ConversationEndOneShot += new Action(MWAXArenaMaster.StartPlayerPracticeAfterConversationEnd);
        }

        private static void StartPlayerPracticeAfterConversationEnd() {
            Mission.Current.SetMissionMode(MissionMode.Battle, false);
            Mission.Current.GetMissionBehaviour<ArenaPracticeFightMissionController>().StartPlayerPractice();
        }

        private void MWAX_game_menu_choose_weapon(MenuCallbackArgs args) {
            MWAXArenaMaster.OpenMissionWithSettingPreviousLocation("center", "arena");
            this._enteredFromMenu = true;
        }

        private bool game_menu_enter_practice_fight_on_condition(MenuCallbackArgs args) {
            Settlement currentSettlement = Settlement.CurrentSettlement;

            ArenaMaster am = (ArenaMaster)Campaign.Current.GetCampaignBehavior<ArenaMaster>();
            FieldInfo knowTournaments = typeof(ArenaMaster).GetField("_knowTournaments", BindingFlags.NonPublic | BindingFlags.Instance);

            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            if (!(bool)knowTournaments.GetValue(am) || knowTournaments.GetValue(am) == null) {
                args.Tooltip = new TextObject("{=Sph9Nliz}You need to learn more about the arena by talking with the arena master.", (Dictionary<string, TextObject>)null);
                args.IsEnabled = false;
                return true;
            }
            if (Hero.MainHero.IsWounded && Campaign.Current.IsMainHeroDisguised) {
                args.Tooltip = new TextObject("{=DqZtRBXR}You are wounded and in disguise.", (Dictionary<string, TextObject>)null);
                args.IsEnabled = false;
                return true;
            }
            if (Hero.MainHero.IsWounded) {
                args.Tooltip = new TextObject("{=yNMrF2QF}You are wounded", (Dictionary<string, TextObject>)null);
                args.IsEnabled = false;
                return true;
            }
            if (Campaign.Current.IsMainHeroDisguised) {
                args.Tooltip = new TextObject("{=jcEoUPCB}You are in disguise.", (Dictionary<string, TextObject>)null);
                args.IsEnabled = false;
                return true;
            }
            if (!currentSettlement.HasTournament)
                return true;
            args.Tooltip = new TextObject("{=NESB0CVc}There is no practice fight because of the Tournament.", (Dictionary<string, TextObject>)null);
            args.IsEnabled = false;
            return true;
        }
    }
}
