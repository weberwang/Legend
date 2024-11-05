using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game.UI.Popups
{
    public class ChoiceSkillPopup : Popup
    {
        [SerializeField] private ChoiceSkillItem[] _choiceSkillItems;
        [SerializeField] private GameObject _rerolls;
        [SerializeField] private SuperTextMesh _rerollsText;

        private UpgradeSkillData[] _upgradeSkillDatas;

        private Hero _hero;

        public override void Show(object data = null)
        {
            base.Show(data);
            _hero = ShortcutPlayer.Get<Hero>();
            _upgradeSkillDatas = data as UpgradeSkillData[];
            UpdateData();
            Game.Instance.PauseGame();
        }

        public override void Hide()
        {
            base.Hide();
            Game.Instance.ResumeGame();
        }

        private void UpdateData()
        {
            for (int i = 0; i < _upgradeSkillDatas.Length; i++)
            {
                if (_choiceSkillItems.Length > i)
                {
                    _choiceSkillItems[i].gameObject.SetActive(true);
                    _choiceSkillItems[i].SetData(_upgradeSkillDatas[i]);
                }
                else
                {
                    _choiceSkillItems[i].gameObject.SetActive(false);
                }
            }

            var rerollsCount = _hero.GetRuntimeStatValue(TraitsID.TRAITS_REROLLS);

            if (rerollsCount > 0)
            {
                _rerolls.SetActive(true);
                _rerollsText.text = string.Format("剩{0}次", rerollsCount);
            }
            else
            {
                _rerolls.SetActive(false);
            }
        }

        public void OnClickRefresh()
        {
            _upgradeSkillDatas = _hero.RefreshChoiceSkill();
            if (_upgradeSkillDatas == null)
            {
                return;
            }

            var rerollsStat = _hero.GetRuntimeStatData(TraitsID.TRAITS_REROLLS);
            rerollsStat.AddModifier(ModifierType.Constant, -1);
            UpdateData();
        }
    }
}