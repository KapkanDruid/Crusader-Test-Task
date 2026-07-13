using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS.Runtime.Components.Audio
{
    [Serializable]
    public class SFXComponent : CMSComponent
    {
        public List<AudioClip> Clips = new();

        [Tooltip("Задержка в секундах перед повторным проигрыванием этого же звука.")]
        [Range(0f, 10f)]
        public float Cooldown = 0.1f;
    }
}
