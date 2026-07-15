using Game.CMS.Runtime.Services.Audio;
using System;
using UnityEngine;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class MuteLayerComponent : CMSComponent
    {
        public MusicLayerType Layer;
        [Range(0, 10)] public float FadeInTime;
    }
}
