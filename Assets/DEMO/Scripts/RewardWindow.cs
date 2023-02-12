using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardWindow : MonoBehaviour
{
	[SerializeField] private Button _backButton;
	[SerializeField] private GameObject _forest;
	[SerializeField] private GameObject _ocean;
	[SerializeField] private GameObject _fire;

	private Dictionary<RewardType, GameObject> _rewards;

	private void Awake()
	{
		_backButton.onClick.AddListener(HideWindow);
	}

	public void Init()
	{
		_rewards = new()
		{
			{RewardType.Forest, _forest},
			{RewardType.Ocean, _ocean},
			{RewardType.Fire, _fire},
		};
	}

	public void ShowWindow(RewardType rewardType)
	{
		foreach (var reward in _rewards)
		{
			reward.Value.SetActive(rewardType == reward.Key);
		}
		gameObject.SetActive(true);
	}

	public void HideWindow()
	{
		gameObject.SetActive(false);
	}
}
