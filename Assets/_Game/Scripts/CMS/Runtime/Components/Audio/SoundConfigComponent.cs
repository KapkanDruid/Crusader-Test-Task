using System;
using UnityEngine;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class SoundConfigComponent : CMSComponent
    {
        [SerializeField, Range(0, 5)] private float _volume = 1f;
        [SerializeField, Range(-3, 3)] private float _minPitch = 1f;
        [SerializeField, Range(-3, 3)] private float _maxPitch = 1f;
        [SerializeField, Range(-1, 1)] private float _stereoPan = 0f;
        [SerializeField, Range(0, 1.1f)] private float _reverbZoneMix = 1f;

        public float Volume => _volume;
        public Vector2 PitchRange => new(_minPitch, _maxPitch);
        public float StereoPan => _stereoPan;
        public float ReverbZoneMix => _reverbZoneMix;
    }
}
