using Game.CMS.Runtime;
using Game.Runtime.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Artifacts
{
    public class ArtifactPopupService
    {
        private readonly ResourcePopupService _resourcePopupService;
        private readonly Dictionary<CMSEntity, RectTransform> _points = new();

        public ArtifactPopupService(ResourcePopupService resourcePopupService)
        {
            _resourcePopupService = resourcePopupService;
        }

        public void Register(CMSEntity artifact, RectTransform point)
        {
            _points[artifact] = point;
        }

        public void Show(CMSEntity artifact, CMSEntity resource, int count)
        {
            if (_points.TryGetValue(artifact, out var point))
                _resourcePopupService.Show(point, resource, count);
        }
    }
}
