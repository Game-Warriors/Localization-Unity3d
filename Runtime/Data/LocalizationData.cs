using GameWarriors.LocalizeDomain.Abstraction;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Data
{
    //[CreateAssetMenu(fileName = "LocalizationData", menuName = "Tools/Localize")]
    public class LocalizationData : ScriptableObject, ILocalizationData
    {
        public const string ASSET_NAME = "LocalizationData_{0}";
        public const string ASSET_DATA_PATH = "Assets/AssetData/Resources/LocalizationData_{0}.asset";
        [SerializeField]
        private List<TermData> _termList;
        //[SerializeField]
        //private TMP_FontAsset[] _fontsAsset;
        public int DataCount => _termList?.Count ?? 0;

        //public TMP_FontAsset[] FontsAsset { get => _fontsAsset; set => _fontsAsset = value; }
        public IEnumerable<(string key, string value)> AllItems
        {
            get
            {
                if (_termList == null)
                    yield break;
                foreach (var item in _termList)
                {
                    yield return (item.Key, item.Content);
                }
            }
        }
        public void FillDicTable(IDictionary<string, string> table)
        {
            table.Clear();
            if (_termList == null)
                return;
            int length = DataCount;
            for (int i = 0; i < length; ++i)
            {
                table.Add(_termList[i].Key, _termList[i].Content);
            }
        }

        public static string GetAssetPath(ELanguageType languageType)
        {
            return string.Format(ASSET_NAME, languageType);
        }

#if UNITY_EDITOR

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

        public void AddTerm(string key, string word)
        {
            int index = -1;
            if (_termList == null)
                _termList = new List<TermData>();
            else
                index = _termList.FindIndex((input) => input.Key == key);

            if (index > -1)
            {
                _termList[index] = new TermData(key, word);
            }
            else
            {
                _termList.Add(new TermData(key, word));
            }
            SortData();
        }

        public bool UpdateTerm(string key, string word)
        {
            bool result = false;
            for (int i = 0; i < _termList.Count; i++)
                if (string.Compare(key, _termList[i].Key) == 0)
                {
                    _termList[i] = new TermData(key, word);
                    result = true;
                    break;
                }
            if (result)
                SortData();
            return result;
        }

        public void RefillData(IEnumerable<(string key, string value)> languageData)
        {
            if (_termList != null)
            {
                _termList.Clear();
                foreach ((string key, string value) in languageData)
                {
                    _termList.Add(new TermData(key, value));
                }
            }
        }

        public void Dispose()
        {
            Resources.UnloadAsset(this);
        }

        //public void SetFontsAsset(TMP_FontAsset[] fontsAsset)
        //{
        //    _fontsAsset = fontsAsset;
        //}

#endif
    }
}
