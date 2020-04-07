using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using SandBox.Source.Towns;

namespace ArenaExpansion {
    [HarmonyPatch(typeof(ArenaMaster), "AddDialogs")]
    class MWAXArenaMasterDialoguePatch {
        private static bool Prefix(ArenaMaster __instance, CampaignGameStarter campaignGameStarter) {
            campaignGameStarter.AddDialogLine("mwax_arena_entered_from_menu", "start", "mwax_arena_weapons_list", "{=mwax_arena_2}Alright, which weapon are you taking?", new ConversationSentence.OnConditionDelegate(MWAXArenaMasterDialoguePatch.MWAX_conversation_choose_weapon_condition), (ConversationSentence.OnConsequenceDelegate)null, 100, (ConversationSentence.OnClickableConditionDelegate)null);
            return true;
        }

        private static bool MWAX_conversation_choose_weapon_condition() {
            return Mission.Current.GetMissionBehaviour<MWAXArenaWeaponSwapLogic>().MWAXEnteredFromMenu;
        }
    }
}
