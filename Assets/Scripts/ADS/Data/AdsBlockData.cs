using UnityEngine;

namespace ADS
{
    [CreateAssetMenu(fileName = "AdsBlockData", menuName = "ADS/AdsBlockData", order = 1)]
    public class AdsBlockData : ScriptableObject
    {
        [SerializeField] private ADSBlockType _type;
        [SerializeField] private string _blockId;
        [SerializeField] private string _keyAndroid;
        [SerializeField] private string _keyIOS;

        public ADSBlockType Type => _type;
        public string Key => GetKey();
        public string Id => _blockId;

        private string GetKey()
        {
#if UNITY_IPHONE
            return _keyIOS;
#else
            return _keyAndroid;
#endif
        }
    }
}