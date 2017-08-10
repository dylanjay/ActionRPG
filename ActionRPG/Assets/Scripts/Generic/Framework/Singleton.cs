using UnityEngine;
using Extensions;

namespace Framework
{
    public abstract class Singleton<T> : MonoBehaviourWrapper where T : MonoBehaviourWrapper
    {
        private static T _instance = null;
        private static GameObject _gameObject = null;
        private static Transform _transform = null;
        private static object _lock = new object();
        private static bool appIsQuitting = false;

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (_instance == null)
                        {
                            Debug.LogError("Singleton " + typeof(T).ToString() + " not found. Please create one in your scene");
                        }
                    }
                    _gameObject = _instance.gameObject;
                    _transform = _instance.transform;
                    DontDestroyOnLoad(_gameObject);
                    return _instance;
                }
            }

            protected set
            {
                if (_instance == null)
                {
                    _instance = value;
                    _gameObject = _instance.gameObject;
                    _transform = _instance.transform;
                    DontDestroyOnLoad(_gameObject);
                }
                else if (_instance != value)
                {
                    Debug.LogError("Tried to make multiple instances of a singleton", _instance);
                }
            }
        }

        void OnDestroy() { }

        public new static GameObject gameObject
        {
            get { return _gameObject; }
        }

        public new static Transform transform
        {
            get { return _transform; }
        }

        void OnApplicationQuit()
        {
            appIsQuitting = true;
        }
    }
}
