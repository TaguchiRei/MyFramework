using Unity.VisualScripting;
using UnityEngine;

namespace GamesKeystoneFramework.Test
{
    public class KeyObservable<T>
    {
        private T _value;

#if UNITY_EDITOR
        private T _oldValue;
#endif

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
#if UNITY_EDITOR
                if (_value != null && _oldValue != null)
                {
                    if (!_value.Equals(_oldValue))
                    {
                        Debug.Log($"(\u25c9ω\u25c9\uff40)<value changed… : {_oldValue} → {_value}");
                        _oldValue = _value;
                    }
                }
                else
                {
                    _oldValue = _value;
                }
#endif
            }
        }

        public KeyObservable(T initialValue = default)
        {
            _value = initialValue;
            _oldValue = initialValue;
        }
        // T → KeyObservable<T> への変換（代入時に使う）
        public static implicit operator KeyObservable<T>(T value)
        {
            return new KeyObservable<T>(value);
        }

        // KeyObservable<T> → T への変換（読み取り時に使う）
        public static implicit operator T(KeyObservable<T> observable)
        {
            return observable.Value;
        }
    }
}