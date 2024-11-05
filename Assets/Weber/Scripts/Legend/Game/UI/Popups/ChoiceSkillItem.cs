using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using UnityEngine.UI;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game.UI.Popups
{
    public class ChoiceSkillItem : MonoBehaviour
    {
        [SerializeField] private Image _bg;
        [SerializeField] private Image _icon;
        [SerializeField] private SuperTextMesh _nameText;
        [SerializeField] private SuperTextMesh _descriptionText;
        [SerializeField] private SuperTextMesh _valueText;
        [SerializeField] private SuperTextMesh _rarityText;

        private UpgradeSkillData _upgradeSkillData;

        public void SetData(UpgradeSkillData upgradeSkillData)
        {
            _upgradeSkillData = upgradeSkillData;
            var skillData = upgradeSkillData.skillData;
            _nameText.text = skillData.skillName;
            _descriptionText.text = skillData.description;
            _icon.sprite = skillData.icon;
            if (upgradeSkillData.skillEffectStatValue != null)
            {
                var skillValue = upgradeSkillData.skillEffectStatValue.value;
                _valueText.text = upgradeSkillData.skillEffectStatValue.changeValueType == ModifierType.Constant ? skillValue.ToString() : skillValue * 100 + "%";
                _rarityText.text = Utils.GetSkillRarity(upgradeSkillData.skillRarity);
            }
            else
            {
                _valueText.text = "";
                _rarityText.text = "";
            }
        }

        public void OnClick()
        {
            ShortcutPlayer.Get<Hero>().LearnSkill(_upgradeSkillData.skillData.ID.ToString(), _upgradeSkillData.skillEffectStatValue);
            PopupManager.HidePopup(PopupName.POPUP_CHOICE_SKILL);
        }
    }
}