using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CMS.Runtime.Utils
{
    [Serializable]
    public sealed class AutomationCurve
    {
        public enum Interpolation
        {
            Linear = 0,
            Smooth = 1,
            Step = 2
        }

        [Serializable]
        public struct Key
        {
            [Range(0f, 1f)]
            public float t01;

            public float value;

            public Key(float t01, float value)
            {
                this.t01 = t01;
                this.value = value;
            }
        }

        [SerializeField]
        private List<Key> _keys = new()
        {
            new Key(0f, 0f),
            new Key(1f, 0f),
        };

        [SerializeField] private float _timeSpan = 1f;
        [SerializeField] private string _timeUnit = "s";
        [SerializeField] private float _valueMin = -80f;
        [SerializeField] private float _valueMax = 0f;
        [SerializeField] private string _valueUnit = "dB";
        [SerializeField] private Interpolation _interpolation = Interpolation.Linear;
        [SerializeField] private bool _clampValue = true;

        public IReadOnlyList<Key> Keys => _keys;

        public float TimeSpan
        {
            get => _timeSpan;
            set => _timeSpan = Mathf.Max(0.0001f, value);
        }

        public string TimeUnit
        {
            get => _timeUnit;
            set => _timeUnit = string.IsNullOrWhiteSpace(value) ? "t" : value;
        }

        public float ValueMin
        {
            get => _valueMin;
            set
            {
                _valueMin = value;
                if (_valueMax < _valueMin) _valueMax = _valueMin;
            }
        }

        public float ValueMax
        {
            get => _valueMax;
            set
            {
                _valueMax = value;
                if (_valueMax < _valueMin) _valueMin = _valueMax;
            }
        }

        public string ValueUnit
        {
            get => _valueUnit;
            set => _valueUnit = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        public Interpolation Mode
        {
            get => _interpolation;
            set => _interpolation = value;
        }

        public bool ClampValue
        {
            get => _clampValue;
            set => _clampValue = value;
        }

        public AutomationCurve(
            float timeSpan = 1f,
            string timeUnit = "s",
            float valueMin = -80f,
            float valueMax = 0f,
            string valueUnit = "dB",
            Interpolation mode = Interpolation.Linear,
            bool clamp = true,
            IEnumerable<Key> keys = null)
        {
            _timeSpan = Mathf.Max(0.0001f, timeSpan);
            _timeUnit = string.IsNullOrWhiteSpace(timeUnit) ? "t" : timeUnit;

            _valueMin = valueMin;
            _valueMax = valueMax;
            if (_valueMax < _valueMin) (_valueMin, _valueMax) = (_valueMax, _valueMin);

            _valueUnit = string.IsNullOrWhiteSpace(valueUnit) ? string.Empty : valueUnit;
            _interpolation = mode;
            _clampValue = clamp;

            _keys = keys != null
                ? new List<Key>(keys)
                : new List<Key>
                {
                    new Key(0f, 0f),
                    new Key(1f, 0f),
                };

            EnsureSortedAndClamped();
        }

        public float GetStartValue()
        {
            return _keys.FirstOrDefault().value;
        }

        public float Evaluate(float t01)
        {
            if (_keys == null || _keys.Count == 0)
                return 0f;

            EnsureSortedAndClamped();
            t01 = Mathf.Clamp01(t01);

            if (_keys.Count == 1)
                return ClampY(_keys[0].value);

            if (t01 <= _keys[0].t01) return ClampY(_keys[0].value);
            if (t01 >= _keys[^1].t01) return ClampY(_keys[^1].value);

            int rightIndex = FindRightKeyIndex(t01);
            int leftIndex = rightIndex - 1;
            Key leftKey = _keys[leftIndex];
            Key rightKey = _keys[rightIndex];

            float deltaT = rightKey.t01 - leftKey.t01;
            if (deltaT <= 1e-6f) return ClampY(rightKey.value);

            float u = (t01 - leftKey.t01) / deltaT;
            Interpolation runtimeMode = _interpolation == Interpolation.Smooth
                ? Interpolation.Linear
                : _interpolation;

            float value = runtimeMode switch
            {
                Interpolation.Step => leftKey.value,
                _ => Mathf.LerpUnclamped(leftKey.value, rightKey.value, u),
            };

            return ClampY(value);
        }

        public float NormalizeTime(float timeInUnits)
        {
            return Mathf.Clamp01(timeInUnits / Mathf.Max(1e-6f, _timeSpan));
        }

        public float FromT01(float t01)
        {
            return Mathf.Clamp01(t01) * _timeSpan;
        }

        public void SetKeys(IEnumerable<Key> keys)
        {
            _keys = keys != null ? new List<Key>(keys) : new List<Key>();
            EnsureSortedAndClamped();
        }

        public void AddKey(float t01, float value)
        {
            _keys ??= new List<Key>();
            _keys.Add(new Key(Mathf.Clamp01(t01), value));
            EnsureSortedAndClamped();
        }

        public void RemoveKeyAt(int index)
        {
            if (_keys == null) return;
            if (index < 0 || index >= _keys.Count) return;
            if (_keys.Count <= 1) return;

            _keys.RemoveAt(index);
            EnsureSortedAndClamped();
        }

        private float ClampY(float value)
        {
            return _clampValue ? Mathf.Clamp(value, _valueMin, _valueMax) : value;
        }

        private int FindRightKeyIndex(float t01)
        {
            int lo = 0;
            int hi = _keys.Count - 1;

            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (_keys[mid].t01 < t01) lo = mid + 1;
                else hi = mid;
            }

            return lo;
        }

        private void EnsureSortedAndClamped()
        {
            _keys ??= new List<Key>();
            _keys.Sort((a, b) => a.t01.CompareTo(b.t01));

            for (int i = 0; i < _keys.Count; i++)
            {
                Key key = _keys[i];
                key.t01 = Mathf.Clamp01(key.t01);
                if (_clampValue) key.value = Mathf.Clamp(key.value, _valueMin, _valueMax);
                _keys[i] = key;
            }

            if (_keys.Count == 0)
            {
                _keys.Add(new Key(0f, _valueMin));
                _keys.Add(new Key(1f, _valueMin));
            }
            else if (_keys.Count == 1)
            {
                _keys.Add(new Key(1f, _keys[0].value));
            }

            const float epsilon = 0.0001f;
            for (int i = 1; i < _keys.Count; i++)
            {
                if (_keys[i].t01 <= _keys[i - 1].t01)
                {
                    Key key = _keys[i];
                    key.t01 = Mathf.Min(1f, _keys[i - 1].t01 + epsilon);
                    _keys[i] = key;
                }
            }
        }
    }
}
