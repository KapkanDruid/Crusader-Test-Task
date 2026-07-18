using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class ItemSpawnPanelConfig : CMSComponent
    {
        [SerializeField] private int _spawnCount;
        [SerializeField] private Color _onEnterColor;

        public int SpawnCount => _spawnCount;
        public Color OnEnterColor => _onEnterColor; 
    }
}
