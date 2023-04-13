using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Utilities
{
    public class ObjectPool<T> where T : UnityEngine.Object
    {
        //hide associated game object when in pool
        public bool HideWhenInPool = true;

        //destroy associated game object when destroyed
        public bool DestroyWithGameObject = true;

        private bool _checkDuplicates = false;

        private int _poolSize;
        private T[] _poolObjs;
        private int _poolIndex = -1;
        private System.Func<T> _objectInstantiator;
        private bool _isClearing;

        private const int DefaultPoolSize = 100;

        public ObjectPool(System.Func<T> instantiator, int size = DefaultPoolSize)
        {
            if (instantiator == null)
            {
                Debug.LogError("Need object instatiator");
                return;
            }

            _objectInstantiator = instantiator;
            _poolSize = size;
            _poolObjs = new T[_poolSize];

            //check for duplicated in editor only
#if UNITY_EDITOR
            _checkDuplicates = true;
#endif
        }

        public void Prefetch(int count = DefaultPoolSize)
        {
            for (int i = 0; i < count; i++)
            {
                Return(InstantiatePoolObject());
            }
        }

        /// <summary>
        /// Draw object from pool
        /// </summary>
        public T Draw()
        {
            if (_poolIndex > -1)
            {
                T obj = _poolObjs[_poolIndex--];

                SetAssociatedGameObjectActive(obj, true);

                return obj;
            }

            //instantiate new object if the pool is empty
            return InstantiatePoolObject();
        }

        /// <summary>
        /// Draws object from pool and return false or instantiate a new object and return true
        /// </summary>
        public T Draw(out bool isNew)
        {
            if (_poolIndex > -1)
            {
                T obj = _poolObjs[_poolIndex--];

                SetAssociatedGameObjectActive(obj, true);

                isNew = false;
                return obj;
            }

            isNew = true;
            return InstantiatePoolObject();
        }

        /// <summary>
        /// Send object back to pool
        /// </summary>
        public void Return(T obj)
        {
            if (_isClearing)
            {
                //we simply destroy an object if Return is called while the pool is clearing
                Destroy(obj);
                return;
            }

            if (_poolIndex + 1 >= _poolSize)
            {
                //we have to destroy an object if there is no more place in the pool
                Debug.LogWarning("Pool for " + obj.name + " is too small");
                Destroy(obj);
                return;
            }

            SetAssociatedGameObjectActive(obj, false);

            //putting the same thing into pool twice is an error
            if (_checkDuplicates)
            {
                for (int i = 0; i <= _poolIndex; i++)
                {
                    if (_poolObjs[i].GetInstanceID() == obj.GetInstanceID())
                    {
                        Debug.LogError("You're adding the same object " + obj.name +
                                       " to pool twice! The pool is now corrupted");
                    }
                }
            }

            _poolObjs[++_poolIndex] = obj;
        }

        /// <summary>
        /// Clear the pool. Pooling objects is no longer working when the pool is cleared
        /// </summary>
        public void Clear()
        {
            _isClearing = true;

            if (_poolIndex == -1) return;

            for (int i = 0; i <= _poolIndex; i++)
            {
                Destroy(_poolObjs[i]);
            }

            _poolIndex = -1;
        }

        private T InstantiatePoolObject()
        {
            return _objectInstantiator.Invoke();
        }

        private void SetAssociatedGameObjectActive(T obj, bool state)
        {
            if (!HideWhenInPool) return;

            GameObject go = GetAssociatedGameObject(obj);
            if (go != null)
            {
                go.SetActive(state);
            }
        }

        private GameObject GetAssociatedGameObject(T obj)
        {
            MonoBehaviour mbh = obj as MonoBehaviour;
            if (mbh != null)
            {
                return mbh.gameObject;
            }

            GameObject go = obj as GameObject;
            return go;
        }

        void Destroy(T obj)
        {
            if (DestroyWithGameObject)
            {
                GameObject go = GetAssociatedGameObject(obj);
                if (go == null)
                {
                    Object.Destroy(obj);
                    return;
                }

                Object.Destroy(go);
                return;
            }

            Object.Destroy(obj);
        }
    }
}
