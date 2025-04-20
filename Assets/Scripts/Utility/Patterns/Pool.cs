using System.Collections.Generic;
using UnityEngine;

namespace Utility.Patterns
{
    [System.Serializable]
    public class Pool<T> where T : Component
    {
        public Transform parent;
        [SerializeField] private T[] prefab;
        private List<T> _items = new List<T>();

        public T[] ActiveItems
        {
            get
            {
                List<T> activeItems = new List<T>();

                for (int i = 0; i < _items.Count; i++)
                    if (_items[i].gameObject.activeSelf)
                        activeItems.Add(_items[i]);

                return activeItems.ToArray();
            }
        }

        public T Get
        {
            get
            {
                for (int i = 0; i < _items.Count; i++)
                    if (_items[i].gameObject && !_items[i].gameObject.activeSelf)
                        return _items[i];


                return AddNewItem();
            }
        }

        private T AddNewItem()
        {
            if (prefab == null) return null;

            var item = GameObject.Instantiate(prefab[Random.Range(0, prefab.Length)], parent);
            item.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            Init(item);
            _items.Add(item);
            item.name = typeof(T) + "_" + _items.Count;
            return item;
        }

        public void DeactivateItems()
        {
            for (var i = 0; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }

        public T GetActive
        {
            get
            {
                T item = Get;

                if (item == null) return null;

                item.gameObject.SetActive(true);
                return item;
            }
        }

        protected virtual void Init(T item)
        {
        }
    }
}