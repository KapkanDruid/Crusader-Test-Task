using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.CMS.Runtime.Components.Commons
{
    [Serializable]
    public class AudioSettingsComponent : CMSComponent
    {
        [Serializable]
        private class GroupValues
        {
            public AudioMixerGroup MixerGroup;
            public string VolumeParameter;
        }

        [SerializeField] private AudioMixerGroup _masterGroup;
        [SerializeField] private AudioMixerGroup _voiceGroup;
        [SerializeField] private AudioMixerGroup _SFXGroup;
        [SerializeField] private AudioMixerGroup _musicGroup;

        [Space]
        [SerializeField] private List<GroupValues> _groupValues;

        public AudioMixerGroup VoiceGroup => _voiceGroup;
        public AudioMixerGroup SFXGroup => _SFXGroup;
        public AudioMixerGroup MusicGroup => _musicGroup;
        public AudioMixerGroup MasterGroup => _masterGroup;

        public string GetVolumeParameter(AudioMixerGroup mixerGroup)
        {
            var groupValue = _groupValues.FirstOrDefault(groupValues => groupValues.MixerGroup == mixerGroup);
            return groupValue == null ? default : groupValue.VolumeParameter;
        }
    }
}
