using Cysharp.Threading.Tasks;

namespace ADS.Factory
{
	public class AdsFactory
	{
		private readonly IAdsManager _adsManager;
		private readonly IAdsAudioManagerProvider _audioManagerProvider;
		private readonly IAdsErrorProvider _errorProvider;

		private bool _inProgress;
		private bool _isOpened;

		public AdsFactory(IAdsManager adsManager, IAdsAudioManagerProvider audioManagerProvider, IAdsErrorProvider errorProvider)
		{
			_adsManager = adsManager;
			_audioManagerProvider = audioManagerProvider;
			_errorProvider = errorProvider;
		}
		
		public async UniTask<bool> CreateAds(string id)
		{
			if (_inProgress)
			{
				return false;
			}
			
			_inProgress = true;
			_isOpened = false;
			
			bool isDone = await _adsManager.TryShowRewardedAds(id, HandleAdsStart, HandleAdsFinish);

			if (!isDone && !_isOpened)
			{
				_errorProvider.HandleErrorAds();
			}

			_inProgress = false;

			return isDone;
		}

		private void HandleAdsStart()
		{
			_isOpened = true;
			_audioManagerProvider.SetActiveAllSounds(false);
		}
		
		private void HandleAdsFinish()
		{
			_audioManagerProvider.SetActiveAllSounds(true);
		}
	}
}