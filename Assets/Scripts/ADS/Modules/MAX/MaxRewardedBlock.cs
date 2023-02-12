using System;
using ADS.Interfaces;
using Cysharp.Threading.Tasks;

namespace ADS.Modules.MAX
{
	public class MaxRewardedBlock : IAdsBlock
	{
		public string Id => _data.Id;
		public bool IsLoaded => _rewardedHandler.IsLoaded(_data.Key);
		public bool IsLoading => _rewardedHandler.IsLoading(_data.Key);

		private readonly AdsBlockData _data;
		private readonly MaxRewardedHandler _rewardedHandler;
		private MaxRewardedHandler.Reward _handler;

		private bool _isApplied;
		private bool _isClosed;
		private Action _onOpened;
		private Action _onClosed;

		public MaxRewardedBlock(AdsBlockData blockData, MaxRewardedHandler rewardedHandler)
		{
			_data = blockData;
			_rewardedHandler = rewardedHandler;

			InitHandler();
			StartLoadAd().Forget();
		}

		private void InitHandler()
		{
			_handler = _rewardedHandler.AddUnitListener(_data.Id,_data.Key);
			_handler.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
			_handler.OnAdClickedEvent += OnRewardedAdClickedEvent;
			_handler.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
			_handler.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
			_handler.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
			_handler.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
		}

		public async UniTask<bool> StartLoadAd()
		{
			bool isLoaded = await _rewardedHandler.LoadAd(_data.Key);
			return isLoaded;
		}

		public async UniTask<bool> TryShowAds(Action onOpened = null, Action onClosed = null)
		{
			_isApplied = false;
			_isClosed = false;
			_onOpened = onOpened;
			_onClosed = onClosed;

			_rewardedHandler.SetEnableListener(_data.Id);

			if (!IsLoaded)
			{
				 var taskLoading = StartLoadAd();
				 var taskDelay = UniTask.Delay(AdsManager.TIME_WAITING_AD_LOADING);

				 await UniTask.WhenAny(taskLoading, taskDelay);
			}

			if (IsLoaded)
			{
				ShowAds();
			}
			else
			{
				return false;
			}
			
			await UniTask.WaitUntil(IsDone);

			return _isApplied;
		}

		private bool IsDone()
		{
			return _isClosed;
		}

		private void ShowAds()
		{
			MaxManager.Log($"Start show Ads: {_data.Key}");
			MaxSdk.ShowRewardedAd(_data.Key);
		}

#region EVENTS

		private void OnRewardedAdDisplayedEvent(MaxSdkBase.AdInfo adInfo)
		{
			_onOpened?.Invoke();
			_onOpened = null;
		}

		private void OnRewardedAdFailedToDisplayEvent(MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
		{
			MaxManager.Log($"<color=red>OnRewardedAdFailedToDisplayEvent</color> ({_data.Id}):\n{errorInfo}!");

			_onOpened = null;
			_onClosed = null;
			_rewardedHandler.SetDisableListener();

			StartLoadAd().Forget();
		}

		private void OnRewardedAdHiddenEvent(MaxSdkBase.AdInfo adInfo)
		{
			MaxManager.Log($"OnRewardedAdHiddenEvent ({_data.Id}):\n{adInfo}!");
			_onClosed?.Invoke();
			_onClosed = null;
			_isClosed = true;
			
			_rewardedHandler.SetDisableListener();
			StartLoadAd().Forget();
		}

		private void OnRewardedAdReceivedRewardEvent(MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
		{
			MaxManager.Log($"<color=green>OnRewardedAdReceivedRewardEvent</color> ({_data.Id}):\n{reward}\n{adInfo}!");
			
			_isApplied = true;
		}

		private void OnRewardedAdRevenuePaidEvent(MaxSdkBase.AdInfo adInfo)
		{
		}

		private void OnRewardedAdClickedEvent(MaxSdkBase.AdInfo adInfo)
		{
		}

		#endregion

		public void Dispose()
		{
			_rewardedHandler.RemoveUnitListener(_data.Id,_data.Key);
			_onOpened = null;
			_onClosed = null;
		}
	}
}