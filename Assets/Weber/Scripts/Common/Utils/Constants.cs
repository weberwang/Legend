using GameCreator.Runtime.Common;

namespace Weber.Scripts.Common.Utils
{
    public class TraitsID
    {
        public const string TRAITS_HEALTH = "Health";
        public const string TRAITS_DAMAGE = "Damage";
        public const string TRAITS_ARMOR = "Armor";
        public const string TRAITS_COOLDOWN = "Cooldown";
        public const string TRAITS_DURATION = "Duration";
        public const string TRAITS_INTERVAL = "Interval";
        public const string TRAITS_MOVE_SPEED = "Move-Speed";
        public const string TRAITS_LEVEL = "Level";
        public const string TRAITS_XP = "XP";
        public const string TRAITS_XP_GAIN = "XP-Gain";
        public const string TRAITS_MAX_HEALTH = "Max-Health";
        public const string TRAITS_LUCK = "Luck";
        public const string TRAITS_CRITICAL = "Critical-Chance";
        public const string TRAITS_CRITICAL_DAMAGE = "Critical-Damage";
        public const string TRAITS_PICK_DISTANCE = "Pick-Distance";
        public const string TRAITS_REROLLS = "Rerolls";
        public const string TRAITS_HEALTH_REGENERATION = "Health-Regeneration";
        public const string TRAITS_SPELL_SIZE = "Spell-Size";
        public const string TRAITS_REVIVE = "Revive";
    }

    public class SignalNames
    {
        public const string SPELL_HIT = "SpellHit";
        public const string SKILL_LEARNED = "SkillLearned";
        public const string ENEMY_DEATH = "EnemyDeath";
        public const string PICK_COIN = "PickCoin";
        public const string PICK_XP = "PickXP";
        public const string LEVEL_UP = "LevelUp";
        public const string PAUSE_GAME = "PauseGame";
        public const string RESUME_GAME = "ResumeGame";
        public const string GAME_START = "GameStart";
        public const string GAME_OVER = "GameOver";
        public const string HURT = "Hurt";
    }

    public class PopupName
    {
        public const string POPUP_PAUSE = "PausePopup";
        public const string POPUP_CHOICE_SKILL = "ChoiceSKillPopup";
    }
}