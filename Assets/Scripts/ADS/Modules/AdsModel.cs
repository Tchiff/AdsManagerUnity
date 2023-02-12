using System;
using ADS.Interfaces;
using Cysharp.Threading.Tasks;

namespace ADS.Modules
{
    public abstract class AdsModel : IAdsModel
    {
        public abstract UniTask Init();
        public abstract UniTask AddBlock(AdsBlockData block);
        public abstract UniTask<bool> TryShowAds(ADSBlockType type, string id, Action onOpened = null, Action onClosed = null);

		public abstract void Dispose();
	}
}