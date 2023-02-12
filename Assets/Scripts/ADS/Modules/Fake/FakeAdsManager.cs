using System;
using ADS.FakeADS;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ADS.Modules.Fake
{
	public class FakeAdsManager : AdsModel
	{
		private readonly RectTransform _root;
		private readonly FakeAdsData _data;
		
		private FakeAdsWindow _window;
		
		public FakeAdsManager(RectTransform root, FakeAdsData data)
		{
			_root = root;
			_data = data;
		}

		public override UniTask<bool> TryShowAds(ADSBlockType type, string id, Action onOpened = null, Action onClosed = null)
		{
			if (_window == null)
			{
				_window = Object.Instantiate(_data.Window, _root);
			}
			return _window.ShowWindow(_data.WaitSeconds, onOpened, onClosed);
		}
		
		public override void Dispose()
		{
			if (_window != null)
			{
				Object.Destroy(_window);
			}
		}
		
		
		public override UniTask Init()
		{
			return UniTask.CompletedTask;
		}
		public override UniTask AddBlock(AdsBlockData block)
		{
			return UniTask.CompletedTask;
		}
	}
}