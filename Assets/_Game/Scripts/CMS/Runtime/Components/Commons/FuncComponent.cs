using System;
using Cysharp.Threading.Tasks;

namespace Game.CMS.Runtime.Components.Commons
{
    [Serializable]
    public class FuncComponent : CMSComponent
    {
        public Func<UniTask> Func;
    }
}
