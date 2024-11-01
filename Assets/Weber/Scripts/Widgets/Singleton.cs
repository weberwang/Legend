using UnityEngine;

namespace Weber.Widgets
{
    //单例类
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] public bool dontDestroyOnLoad = true;
        private static T instance;

        public static T Instance => instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                if (dontDestroyOnLoad)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            OnCreate();
        }

        protected virtual void OnCreate()
        {
        }
    }
}