using UnityEngine;

namespace ADS.FakeADS
{
	[CreateAssetMenu(fileName = "FakeAdsData", menuName = "ADS/FakeAdsData")]
	public class FakeAdsData : ScriptableObject
	{
		[SerializeField] private FakeAdsWindow _window;
		[SerializeField] private int _waitSeconds = 30;

		public FakeAdsWindow Window => _window;
		public int WaitSeconds => _waitSeconds;
	}
}