using System;
using System.Collections.Generic;
using ADS.FakeADS;
using ADS.Modules;
using ADS.Modules.Fake;
using ADS.Modules.MAX;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ADS
{
    public class AdsManager : IAdsManager
    {
        public const int TIME_WAITING_AD_LOADING = 3000;
        public const int TIME_WAITING_INIT_MODULE = 10000;
        
        public bool IsInitialize { get; private set; }
        
        private readonly Dictionary<AdsType, AdsModel> _models = new();

        public async UniTask InitAds(List<AdsData> adsList)
        {
            foreach (var ads in adsList)
            {
                if (ads.IsEnabled)
                {
                    await AddBlocks(ads);
                }
            }
            IsInitialize = true;
        }

        public void InitFake(RectTransform root, FakeAdsData data)
        {
            var model = new FakeAdsManager(root, data);
            _models[AdsType.Fake] = model;
        }

        public async UniTask<bool> TryShowRewardedAds(string id, Action onOpened = null, Action onClosed = null)
        {
            var module = TryGetAvailableModule();
            if (module != null)
            {
                return await module.TryShowAds(ADSBlockType.Rewarded, id, onOpened, onClosed);
            }
            
            return false;
        }
        
        public async UniTask<bool> TryShowInterstitialAds(string id, Action onOpened = null)
        {
            var module = TryGetAvailableModule();
            if (module != null)
            {
                return await module.TryShowAds(ADSBlockType.Interstitial, id, onOpened);
            }
            
            return false;
        }

        private async UniTask AddBlocks(AdsData data)
        {
            AdsModel model = await TryCreateAndGetModel(data.ADS);
            foreach (var block in data.Blocks)
            {
                model.AddBlock(block);
            }
        }

        private AdsModel TryGetAvailableModule()
        {
            if (_models.ContainsKey(AdsType.MAX))
            {
                return _models[AdsType.MAX];
            }
            
            if (_models.ContainsKey(AdsType.Fake))
            {
                return _models[AdsType.Fake];
            }

            return null;
        }

        private async UniTask<AdsModel> TryCreateAndGetModel(AdsType type)
        {
            if (!_models.ContainsKey(type))
            {
                await CreateModel(type);
            }
            return _models[type];
        }

        private async UniTask CreateModel(AdsType type)
        {
            AdsModel model = null;
            switch (type)
            {
                case AdsType.MAX:
                    model = new MaxManager();
                    break;
                case AdsType.Fake:
                    throw new Exception("Fake ADS need create in InitFake!");
            }
            
            if (model == null)
            {
                throw new Exception($"Cannot create ADS model by type {type}");
            }
            
            _models[type] = model;
            await model.Init();
        }

        public void Dispose()
        {
            foreach (var model in _models.Values)
            {
                model.Dispose();
            }
        }
    }
}