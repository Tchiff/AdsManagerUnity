using System.Collections;
using System.Collections.Generic;
using ADS;
using ADS.FakeADS;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private RectTransform _rootWindows;
    [SerializeField] private FakeAdsData _fakeAdsData;
    [SerializeField] private AdsData _adsData;
    
    [Header("Windows")]
    [SerializeField] private MenuWindow _menuWindow;
    [SerializeField] private RewardWindow _rewardWindow;

    private AdsManager _adsManager;
    
    IEnumerator Start()
    {
        _adsManager = new AdsManager();
        yield return _adsManager.InitAds(new List<AdsData>{_adsData});
        _adsManager.InitFake(_rootWindows, _fakeAdsData);
        
        _rewardWindow.Init();
        _menuWindow.Init(_adsManager,_rewardWindow);
    }
}
