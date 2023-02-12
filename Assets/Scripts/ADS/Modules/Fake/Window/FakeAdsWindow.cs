using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADS.FakeADS
{
	public class FakeAdsWindow : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _timer;
		[SerializeField] private Button _button;
		[SerializeField] private Color _normalTimerColor = Color.white;
		[SerializeField] private Color _completedTimerColor = Color.green;

		private int _waitSeconds;
		private bool _isComplete;
		private bool _inProgress;
		private Action _onClosed;
		
		private void Awake()
		{
			_button.onClick.AddListener(OnClose);
		}

		public async UniTask<bool> ShowWindow(int waitSeconds, Action onOpened = null, Action onClosed = null)
		{
			if (_inProgress)
			{
				return false;
			}
			
			_isComplete = false;
			_inProgress = true;
			_onClosed = onClosed;
			_waitSeconds = waitSeconds;
			ShowWindow(onOpened);

			await WaitTime();
			await UniTask.WaitUntil(()=>!_inProgress);
			
			return _isComplete;
		}

		private async UniTask WaitTime()
		{
			var time = 0;

			while (_inProgress && time < _waitSeconds)
			{
				PrintTime(time);
				await UniTask.Delay(1000);
				time++;
			}

			_isComplete = _inProgress;

			if (_isComplete)
			{
				PrintTime(_waitSeconds);
				_timer.color = _completedTimerColor;
			}
		}
		
		private void OnClose()
		{
			_inProgress = false;
			HideWindow();
		}

		private void PrintTime(int time)
		{
			int revertTime = _waitSeconds - time;
			int min = revertTime / 60;
			int sec = revertTime - (min * 60);
			_timer.text = $"{min:00}:{sec:00}";
		}

		private void HideWindow()
		{
			gameObject.SetActive(false);
			_onClosed?.Invoke();
		}

		private void ShowWindow(Action onOpened)
		{
			_timer.color = _normalTimerColor;
			gameObject.SetActive(true);
			transform.SetAsLastSibling();
			onOpened?.Invoke();
		}
	}
}