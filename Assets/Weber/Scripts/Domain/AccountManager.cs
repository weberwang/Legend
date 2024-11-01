using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Domain
{
    public class AccountManager : Singleton<AccountManager>
    {
        public Account Account { get; private set; }

        private const string SAVE_KEY = "Account";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                Account = ES3.Load<Account>(SAVE_KEY);
            }
            else
            {
                Account = new Account();
            }
        }

        public void AddCoin(int coin)
        {
            Account.coin += coin;
        }
    }

    public class Account
    {
        internal int coin;
    }
}