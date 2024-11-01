using UnityEngine;
using Weber.Scripts.Domain;

namespace Weber.Scripts.Legend.Game.Items
{
    public class CoinDropItem : DropItem
    {
        protected override void OnHeroPicked()
        {
            Game.Instance.PickCoin(1);
        }
    }
}