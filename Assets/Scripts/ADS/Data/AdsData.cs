using System.Collections.Generic;
using UnityEngine;

namespace ADS
{
    [CreateAssetMenu(fileName = "AdsData", menuName = "ADS/AdsData", order = 0)]
    public class AdsData : ScriptableObject
    {
        [SerializeField] bool _isEnabled = true;
        [SerializeField] AdsType _ads;
        [SerializeField] List<AdsBlockData> _blocks;

        public bool IsEnabled
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
                return  _isEnabled;
#else
                return false;
#endif
			}
#if UNITY_EDITOR
            set
            {
                _isEnabled = value;
            }
#endif
        }
		
        public AdsType ADS => _ads;
        public List<AdsBlockData> Blocks => _blocks;
    }
}