using UnityEngine;

namespace Utility.Patterns
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T instance;
        public static T Instance => instance ?? InitInstance();

        private static T InitInstance()
        {
            CreateInstance();
            instance.Initialize();
            return instance;
        }

        private static void CreateInstance(GameObject prefab = null, bool ifExistRecreate = false)
        {
            if (!instance)
                instance = GameObject.FindObjectOfType<T>();

            if (!ifExistRecreate && instance)
                return;
            else if (instance)
                Destroy(instance.gameObject);

            if (prefab)
            {
                instance = Instantiate(prefab).GetComponent<T>();
                instance.Initialize();
                return;
            }

            instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}