using HarmonyLib;
using SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.MountAndBlade;

namespace ArenaExpansion {
    [HarmonyPatch(typeof(ArenaMasterCampaignBehavior), "AddDialogs")]
    class MWAXArenaMasterDialoguePatch {
        private static bool Prefix(CampaignGameStarter campaignGameStarter) {
            campaignGameStarter.AddDialogLine("mwax_arena_entered_from_menu", "start", "mwax_arena_weapons_list", "{=mwax_arena_2}Alright, which weapon are you taking?", new ConversationSentence.OnConditionDelegate(MWAXArenaMasterDialoguePatch.MWAX_conversation_choose_weapon_condition), (ConversationSentence.OnConsequenceDelegate)null, 100, (ConversationSentence.OnClickableConditionDelegate)null);
            return true;
        }

        private static bool MWAX_conversation_choose_weapon_condition() {
            if (Mission.Current.GetMissionBehavior<MWAXArenaWeaponSwapLogic>() == null)
                return false;

            return Mission.Current.GetMissionBehavior<MWAXArenaWeaponSwapLogic>().MWAXEnteredFromMenu;
        }
    }
}
