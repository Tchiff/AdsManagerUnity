using System;
using Cysharp.Threading.Tasks;

namespace ADS.Interfaces
{
    public interface IAdsModel : IDisposable
    {
        UniTask<bool> TryShowAds(ADSBlockType type, string id, Action onOpened = null, Action onClosed = null);
    }
}