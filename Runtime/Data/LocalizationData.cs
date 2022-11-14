using System.Collections.Generic;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Data
{
    //[CreateAssetMenu(fileName = "LocalizationData", menuName = "Tools/Localize")]
    public class LocalizationData : ScriptableObject
    {
        public const string ASSET_NAME = "LocalizationData";
        public const string ASSET_DATA_PATH = "Assets/AssetData/Resources/LocalizationData.asset";
        [SerializeField]
        private List<Term> _termList;
        //[SerializeField]
        //private TMP_FontAsset[] _fontsAsset;
        public int DataCount => _termList?.Count ?? 0;

        //public TMP_FontAsset[] FontsAsset { get => _fontsAsset; set => _fontsAsset = value; }

        public void FillDicTable(Dictionary<string, string[]> table)
        {
            table.Clear();
            if (_termList == null)
                return;
            int length = DataCount;
            for (int i = 0; i < length; ++i)
            {
                table.Add(_termList[i].Key, _termList[i].Contents);
            }
        }

#if UNITY_EDITOR

        public void AddTerm(Term term)
        {

            int index = -1;
            if (_termList == null)
                _termList = new List<Term>();
            else
                index = _termList.FindIndex((input) => input.Key == term.Key);

            if (index > -1)
            {
                _termList[index] = term;
            }
            else
            {
                _termList.Add(term);
            }
            SortData();
        }

        public bool UpdateTerm(Term term)
        {
            bool result = false;
            for (int i = 0; i < _termList.Count; i++)
                if (string.Compare(term.Key, _termList[i].Key) == 0)
                {
                    _termList[i] = term;
                    result = true;
                    break;
                }
            if (result)
                SortData();
            return result;
        }

        public void RemoveTerm(string key)
        {
            int index = _termList.FindIndex(input => string.Compare(input.Key, key) == 0);
            _termList.RemoveAt(index);
        }

        public string GetTermKeyAt(int index)
        {
            if (_termList == null || _termList.Count <= index)
                return null;
            return _termList[index].Key;
        }

        private void SortData()
        {
            _termList.Sort();
        }

        //public void SetFontsAsset(TMP_FontAsset[] fontsAsset)
        //{
        //    _fontsAsset = fontsAsset;
        //}

#endif
    }
}
