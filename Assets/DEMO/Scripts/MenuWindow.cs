using System;
using System.Collections;
using System.Collections.Generic;
using ADS;
using ADS.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
	[SerializeField] private GameObject _buttonsContainer;
	[SerializeField] private Button _oceanReward;
	[SerializeField] private Button _forestReward;
	[SerializeField] private Button _fireReward;

	private AdsFactory _adsFactory;
	private RewardWindow _rewardWindow;
	
	private void Awake()
	{
		_oceanReward.onClick.AddListener(()=>HandleRewardClick(RewardType.Ocean).Forget());
		_fireReward.onClick.AddListener(()=>HandleRewardClick(RewardType.Fire).Forget());
		_forestReward.onClick.AddListener(()=>HandleRewardClick(RewardType.Forest).Forget());
		_buttonsContainer.SetActive(false);
	}

	public void Init(IAdsManager adsManager, RewardWindow rewardWindow)
	{
		var dummy = new DummyAds();
		_adsFactory = new AdsFactory(adsManager,dummy,dummy);
		_rewardWindow = rewardWindow;
		_buttonsContainer.SetActive(true);
	}

	private async UniTask HandleRewardClick(RewardType rewardType)
	{
		var isReward = await _adsFactory.CreateAds("reward");

		if (isReward)
		{
			_rewardWindow.ShowWindow(rewardType);
		}
	}
}
