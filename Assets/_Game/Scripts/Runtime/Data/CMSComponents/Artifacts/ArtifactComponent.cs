using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class ArtifactComponent : CMSComponent
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private string _name;
        [SerializeField, TextArea] private string _description;

        public Sprite Sprite => _sprite;
        public string Name => _name;
        public string Description => _description;
    }
}
