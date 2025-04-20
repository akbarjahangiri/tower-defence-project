using UnityEngine;

namespace Utility.Patterns
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly object _lock = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            Debug.Log($"Create instance of {typeof(T).Name}");
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }

        public static void Reset()
        {
            if (_instance is Singleton<T> singleton)
            {
                singleton.OnReset();
            }

            _instance = null;
        }

        protected virtual void OnReset()
        {
        }

        protected Singleton()
        {
        }
    }
}