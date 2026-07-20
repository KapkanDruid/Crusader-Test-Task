using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class ProductionPerCellArtifactComponent : CMSComponent
    {
        [SerializeField] private CMSAsset _resource;
        [SerializeField] private int _countPerCell;

        public CMSAsset Resource => _resource;
        public int CountPerCell => _countPerCell;
    }
}
