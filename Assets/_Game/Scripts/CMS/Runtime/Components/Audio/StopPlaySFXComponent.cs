using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class StopPlaySFXComponent : CMSComponent
    {
        public List<AudioClip> Clips = new();
    }
}
