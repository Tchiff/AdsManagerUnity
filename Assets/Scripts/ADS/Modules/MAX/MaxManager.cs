using System;
using System.Collections.Generic;
using System.Linq;
using ADS.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ADS.Modules.MAX
{
	public class MaxManager : AdsModel
	{
		private Dictionary<ADSBlockType, List<IAdsBlock>> _blocks = new();
		private bool Inited;
		private MaxSdkBase.SdkConfiguration _config;

		private const string LOG_KEY = "MAX";
		private const string SDK_LEY = "vSfzs3bbK48C3itHm2HQ3wM2eeULfzTpnsLSzE-c72-RaKkERIR5oKkV6ezlHpPw1Ga80lRk9MDrgm3__Nw2T0";

		public MaxRewardedHandler RewardedHandler
		{
			get
			{
				if (_rewardedHandler == null)
				{
					_rewardedHandler = new MaxRewardedHandler();
				}
				return _rewardedHandler;
			}
		}
		private MaxRewardedHandler _rewardedHandler;

		public override async UniTask Init()
		{
			if (MaxSdk.IsInitialized())
			{
				Log("Already Initialize!");
				_config = MaxSdk.GetSdkConfiguration();
				Inited = true;
				return;
			}

			MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
			MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;
#if !DEV
			MaxSdk.SetCreativeDebuggerEnabled(false);
#endif
			Log($"Start Initialize!");
			MaxSdk.SetSdkKey(SDK_LEY);
			MaxSdk.InitializeSdk();
			
			var taskInit = UniTask.WaitUntil(() => Inited);
			var taskDelay = UniTask.Delay(AdsManager.TIME_WAITING_INIT_MODULE);
			await UniTask.WhenAny(taskInit, taskDelay);

			if (!Inited)
			{
				LogError($"[{LOG_KEY}] The init request timed out!");
			}
		}

		private void OnSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
		{
			Log($"Sdk Initialized!");
			_config = sdkConfiguration;
			Inited = true;
#if DEV
			Log($"Show Debugger!");
			MaxSdk.ShowMediationDebugger();
#endif
		}

		public override async UniTask AddBlock(AdsBlockData blockData)
		{
			await UniTask.WaitUntil(() => Inited);
			
			Log($"Add Block {blockData.Id}!");
				
			if (!_blocks.ContainsKey(blockData.Type))
			{
				_blocks.Add(blockData.Type, new List<IAdsBlock>());
			}

			IAdsBlock block = CreateBlock(blockData);
			if (block != null)
			{
				_blocks[blockData.Type].Add(block);
			}
		}

		public override async UniTask<bool> TryShowAds(ADSBlockType type, string id, Action onOpened = null, Action onClosed = null)
		{
			if (_config == null || !_config.IsSuccessfullyInitialized || !_blocks.ContainsKey(type) || _blocks[type].Count <= 0)
			{
				LogError($"Cannot show ads {type}:{id}! _config == null:{_config == null}; IsSuccessfullyInitialized:{_config != null && _config.IsSuccessfullyInitialized}; !_blocks.ContainsKey({type}):{!_blocks.ContainsKey(type)}");
				return false;
			}

			Log($"Try Show Ads {type}:{id}!");
			
			var actualBlocks = _blocks[type].Where(block => block.Id == id).ToList();

			if (actualBlocks.Count <= 0)
			{
				LogError($"Not found ads block {type}:{id}!");
				return false;
			}

			foreach (var block in actualBlocks)
			{
				if (block.IsLoaded)
				{
					Log($"Show loaded block: {block.Id}!");
					return await block.TryShowAds(onOpened, onClosed);
				}
			}

			bool isLoaded = false;
			foreach (var block in actualBlocks)
			{
				if (_config == null)
				{
					Log($"Break show Ads {type}:{id}!");
					return false;
				}

				if (isLoaded)
				{
					block.StartLoadAd().Forget();
					continue;
				}

				Log($"Try load block: {block.Id}!");
				isLoaded = await block.TryShowAds(onOpened, onClosed);
			}
			
			return isLoaded;
		}

		public override void Dispose()
		{
			_config = null;
			
			MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
			
			foreach (var blocks in _blocks.Values)
			{
				foreach (var block in blocks)
				{
					block.Dispose();
				}
			}
			_blocks.Clear();
			
			_rewardedHandler?.Dispose();
			_rewardedHandler = null;
		}

		private IAdsBlock CreateBlock(AdsBlockData blockData)
		{
			switch (blockData.Type)
			{
				case ADSBlockType.Rewarded:
					return new MaxRewardedBlock(blockData, RewardedHandler);
				default:
					return null;
			}
		}
		
		
		public static void Log(string message)
		{
#if SHOW_LOGS
			Debug.Log($"[{LogKey}] {message}");
#endif
		}
		
		public static void LogError(string message)
		{
			Debug.LogError($"[{LOG_KEY}] {message}");
		}
	}
}