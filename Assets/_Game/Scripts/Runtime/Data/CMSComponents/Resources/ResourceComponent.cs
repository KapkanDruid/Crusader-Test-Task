using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Resources
{
    [Serializable]
    public class ResourceComponent : CMSComponent
    {
        [SerializeField] private Sprite _sprite;

        public Sprite Sprite => _sprite; 
    }
}
