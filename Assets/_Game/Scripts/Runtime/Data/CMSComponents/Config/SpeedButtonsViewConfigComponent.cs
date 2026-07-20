using Game.CMS.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class SpeedButtonsViewConfigComponent : CMSComponent
    {
        [SerializeField] private List<float> _speedValues = new();
        [SerializeField] private GameObject _speedButtonPrefab;

        public IReadOnlyList<float> SpeedValues => _speedValues;
        public GameObject SpeedButtonPrefab => _speedButtonPrefab;
    }
}
