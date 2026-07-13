using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class VoiceComponent : CMSComponent
    {
        public AudioClip Clip;
        public LocalizedAudioClip localizeClip;
        [Range(0, 2)] public float Volume = 1f;
    }
}
