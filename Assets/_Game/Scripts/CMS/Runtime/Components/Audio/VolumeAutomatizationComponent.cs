using Game.CMS.Runtime.Utils;
using System;
using UnityEngine.Audio;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class VolumeAutomatizationComponent : CMSComponent
    {
        public AudioMixerGroup MixerGroup;
        public bool FromCurrentVolume;
        public AutomationCurve VolumeCurve = new AutomationCurve(valueMax: 20, valueMin: -80);
    }
}
