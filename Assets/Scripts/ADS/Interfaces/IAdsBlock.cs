using System;
using Cysharp.Threading.Tasks;

namespace ADS.Interfaces
{
    public interface IAdsBlock : IDisposable
    {
	    string Id { get; }
        bool IsLoaded { get; }
        UniTask<bool> TryShowAds(Action onOpened = null, Action onClosed = null);
		UniTask<bool> StartLoadAd();
    }
}