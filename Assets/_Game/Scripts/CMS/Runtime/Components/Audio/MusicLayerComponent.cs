using Game.CMS.Runtime.Services.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class MusicLayerComponent : CMSComponent
    {
        [Serializable]
        public class LayerData
        {
            public MusicLayerType Layer;
            public AudioClip LayerClip;
            public bool IsMuteOnStart;
        }

        public List<LayerData> Layers;
    }
}
