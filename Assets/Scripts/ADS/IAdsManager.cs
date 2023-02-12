using System;
using Cysharp.Threading.Tasks;

namespace ADS
{
	public interface IAdsManager
	{
		bool IsInitialize { get; }
		UniTask<bool> TryShowRewardedAds(string id, Action onOpened = null, Action onClosed = null);
		UniTask<bool> TryShowInterstitialAds(string id, Action onOpened = null);
		void Dispose();
	}
}