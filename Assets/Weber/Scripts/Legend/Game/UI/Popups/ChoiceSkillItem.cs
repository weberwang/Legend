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
            _icon.sprite = skillData.icon;
            _descriptionText.text = skillData.description;
            if (upgradeSkillData.skillEffectStatValue != null)
            {
                //提升已经学习的技能
                var skillEffectStatValue = upgradeSkillData.skillEffectStatValue;
                var value = skillEffectStatValue.value;
                var valueString = skillEffectStatValue.changeValueType == ModifierType.Constant ? value.ToString() : skillEffectStatValue.value * 100 + "%";
                if (skillData.classType == ClassType.Base)
                {
                    _valueText.text = value > 0 ? "+" + valueString : valueString;
                }
                else
                {
                    _valueText.text = skillEffectStatValue.stat.GetName(null) + " " + (value > 0 ? "+" + valueString : valueString);
                }

                _rarityText.text = Utils.GetSkillRarity(upgradeSkillData.skillRarity);
            }
            else
            {
                _valueText.text = "";
                _rarityText.text = Utils.GetSkillRarity(upgradeSkillData.skillRarity);
            }
        }

        public void OnClick()
        {
            ShortcutPlayer.Get<Hero>().LearnSkill(_upgradeSkillData.skillData.ID.ToString(), _upgradeSkillData.skillEffectStatValue);
            PopupManager.HidePopup(PopupName.POPUP_CHOICE_SKILL);
        }
    }
}