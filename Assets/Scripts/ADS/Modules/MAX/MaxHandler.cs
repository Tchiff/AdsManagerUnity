using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ADS.Modules.MAX
{
	public interface IAdHandler
	{
		string Id { get; }
	}
	
	public abstract class MaxHandler<T> where T : IDisposable, IAdHandler
	{
		private readonly Dictionary<string,  Dictionary<string, T>> _listener = new Dictionary<string,  Dictionary<string, T>>();
		protected readonly HashSet<string> _loading = new HashSet<string>();
		private string _currentListener = string.Empty;

		protected abstract T CreateListener(string id);

		protected bool TryGetListener(string adUnitId, out T listener)
		{
			if (_listener.ContainsKey(adUnitId))
			{
				var collection = _listener[adUnitId];

				if (collection.ContainsKey(_currentListener))
				{
					listener = collection[_currentListener];
					return true;
				}
			}
			
			MaxManager.LogError($"Cannot get Max Listener with adUnitId '{adUnitId}'. Current Listener = '{_currentListener}'");
			listener = default;
			return false;
		}
		
		public T AddUnitListener(string id, string adUnitId)
		{
			if (!_listener.ContainsKey(adUnitId))
			{
				_listener[adUnitId] =  new Dictionary<string, T>();
			}
			
			if (_listener[adUnitId].ContainsKey(id))
			{
				MaxManager.LogError($"Key {id} already exist Max Listeners!");
				return _listener[adUnitId][id];
			}

			var listener = CreateListener(id);
			_listener[adUnitId].Add(id, listener);
			return listener;
		}
		
		public void RemoveUnitListener(string id, string adUnitId)
		{
			if (_listener.ContainsKey(adUnitId) && _listener[adUnitId].ContainsKey(id))
			{
				_listener[adUnitId][id].Dispose();
				_listener[adUnitId].Remove(id);
			}

			_loading.Remove(adUnitId);
		}

		public void SetEnableListener(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				MaxManager.LogError($"Listener is EMPTY id!");
				SetDisableListener();
				return;
			}
			
			_currentListener = id;
		}
		
		public void SetDisableListener()
		{
			_currentListener = string.Empty;
		}
		
		public async UniTask<bool> LoadAd(string adUnitId)
		{
			if (IsLoaded(adUnitId))
			{
				return true;
			}
			
			if (IsLoading(adUnitId))
			{
				MaxManager.Log($"Wait Loading Ads {adUnitId}!");
				await UniTask.WaitUntil(() => !IsLoading(adUnitId));
				return IsLoaded(adUnitId);
			}

			MaxManager.Log($"Start Load Ads {adUnitId}!");
			_loading.Add(adUnitId);
			MaxSdk.LoadRewardedAd(adUnitId);
			await UniTask.WaitUntil(() => !IsLoading(adUnitId));

			return IsLoaded(adUnitId);
		}

		public bool IsLoaded(string adUnitId)
		{
			return MaxSdk.IsRewardedAdReady(adUnitId);
		}

		public bool IsLoading(string adUnitId)
		{
			return _loading.Contains(adUnitId);
		}
	}
}