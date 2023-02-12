using System;

namespace ADS.Modules.MAX
{
	public class MaxRewardedHandler : MaxHandler<MaxRewardedHandler.Reward>
	{
		public class Reward : IDisposable, IAdHandler
		{
			public string Id { get; }
			
			public event Action<MaxSdkBase.AdInfo> OnAdDisplayedEvent;
			public event Action<MaxSdkBase.ErrorInfo, MaxSdkBase.AdInfo> OnAdDisplayFailedEvent;
			public event Action<MaxSdkBase.AdInfo> OnAdRevenuePaidEvent;
			public event Action<MaxSdkBase.AdInfo> OnAdHiddenEvent;
			public event Action<MaxSdk.Reward, MaxSdkBase.AdInfo> OnAdReceivedRewardEvent;
			public event Action<MaxSdkBase.AdInfo> OnAdClickedEvent;

			public Reward(string id)
			{
				Id = id;
			}

			public void OnRewardedAdDisplayedEvent(MaxSdkBase.AdInfo adInfo)
			{
				OnAdDisplayedEvent?.Invoke(adInfo);
			}

			public void OnRewardedAdFailedToDisplayEvent(MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
			{
				OnAdDisplayFailedEvent?.Invoke(errorInfo,adInfo);
			}

			public void OnRewardedAdClickedEvent(MaxSdkBase.AdInfo adInfo)
			{
				OnAdClickedEvent?.Invoke(adInfo);
			}

			public void OnRewardedAdHiddenEvent(MaxSdkBase.AdInfo adInfo)
			{
				OnAdHiddenEvent?.Invoke(adInfo);
			}

			public void OnRewardedAdReceivedRewardEvent(MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
			{
				OnAdReceivedRewardEvent?.Invoke(reward,adInfo);
			}

			public void OnRewardedAdRevenuePaidEvent(MaxSdkBase.AdInfo adInfo)
			{
				OnAdRevenuePaidEvent?.Invoke(adInfo);
			}

			public void Dispose()
			{
				OnAdDisplayedEvent = null;
				OnAdClickedEvent = null;
				OnAdRevenuePaidEvent = null;
				OnAdHiddenEvent = null;
				OnAdDisplayFailedEvent = null;
				OnAdReceivedRewardEvent = null;
			}
		}

		public MaxRewardedHandler()
		{
			MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
			MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
			MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
			MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
		}

		private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			MaxManager.Log($"<color=green>OnRewardedAdLoadedEvent</color>:\n{adInfo.AdUnitIdentifier}!");
			_loading.Remove(adUnitId);
		}

		private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
			MaxManager.Log($"<color=red>OnRewardedAdLoadFailedEvent</color>:\n{errorInfo.Message}!");
			_loading.Remove(adUnitId);
		}

		private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			if (TryGetListener(adUnitId, out Reward reward))
			{
				reward.OnRewardedAdDisplayedEvent(adInfo);
			}
		}

		private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
		{
			_loading.Remove(adUnitId);
			
			if (TryGetListener(adUnitId, out Reward reward))
			{
				reward.OnRewardedAdFailedToDisplayEvent(errorInfo,adInfo);
			}
		}

		private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			if (TryGetListener(adUnitId, out Reward reward))
			{
				reward.OnRewardedAdClickedEvent(adInfo);
			}
		}

		private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			if (TryGetListener(adUnitId, out Reward reward))
			{
				reward.OnRewardedAdHiddenEvent(adInfo);
			}
		}

		private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
		{
			if (TryGetListener(adUnitId, out Reward info))
			{
				info.OnRewardedAdReceivedRewardEvent(reward, adInfo);
			}
		}

		private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			if (TryGetListener(adUnitId, out Reward info))
			{
				info.OnRewardedAdRevenuePaidEvent(adInfo);
			}
		}

		protected override Reward CreateListener(string id)
		{
			return new Reward(id);
		}

		public void Dispose()
		{
			MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
			MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaidEvent;
			MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
			MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
		}
	}
}