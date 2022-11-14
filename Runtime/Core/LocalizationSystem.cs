using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using GameWarriors.LocalizeDomain.Abstraction;
using GameWarriors.LocalizeDomain.Data;
using GameWarriors.LocalizeDomain.Extensions;
using System.Threading.Tasks;
using System.Collections;

namespace GameWarriors.LocalizeDomain.Core
{
    public class LocalizationSystem : ILocalize
    {
        private const string LANGUAGE_CACH_ID = "LCID";

        public event Action OnLanguageChanged;
        private LocalizationData _dataCollection;
        private ELanguageType _currentLanguage;
        private Dictionary<string, string[]> _termsDictionary;
        //private TMP_FontAsset[] _fontsAsset;

        public bool IsRTL => CurrentLanguage == ELanguageType.Fa || CurrentLanguage == ELanguageType.Ar;
        public ELanguageType CurrentLanguage => _currentLanguage;

        public LocalizationSystem(ILocalizeResource localizeResource)
        {
            if (localizeResource == null)
                localizeResource = new DefaultLocalizeLoader();

            int cacheLanguageId = PlayerPrefs.GetInt(LANGUAGE_CACH_ID, -1);
            if (cacheLanguageId == -1)
            {
                _currentLanguage = ELanguageType.En;
            }
            else
                _currentLanguage = (ELanguageType)cacheLanguageId;
            PlayerPrefs.SetInt(LANGUAGE_CACH_ID, (int)_currentLanguage);

            localizeResource.LoadResourceAsync(LocalizationData.ASSET_NAME, LoadData);
        }

        [UnityEngine.Scripting.Preserve]
        public async Task WaitForLoading()
        {
            while (_termsDictionary == null)
            {
                await Task.Delay(100);
            }
        }

        [UnityEngine.Scripting.Preserve]
        public IEnumerator WaitForLoadingCoroutine()
        {
            yield return new WaitUntil(() => _termsDictionary != null);
        }

        public void SetLanguage(ELanguageType language)
        {
            if (_currentLanguage != language)
            {
                _currentLanguage = language;
                PlayerPrefs.SetInt(LANGUAGE_CACH_ID, (int)language);
                PlayerPrefs.Save();
                OnLanguageChanged?.Invoke();
            }
        }

        //public TMP_FontAsset GetFontAsset(ELanguageType languageType)
        //{
        //    int index = (int)languageType;
        //    return _fontsAsset[index];
        //}

        public Sprite GetLocalizedSprite(string key)
        {
            throw new NotImplementedException();
        }

        public string GetTermTranslation(string term)
        {
            string content = _termsDictionary.Translate(term, CurrentLanguage);
            //if (IsRTL && !string.IsNullOrEmpty(content))
            //    content = ApplyRTLfix(content, 0, true);

            return content;
        }

        public string GetTermTranslation(string term, ELanguageType languageType)
        {
            string content = _termsDictionary.Translate(term, languageType);
            //if (IsRTL && !string.IsNullOrEmpty(content))
            //    content = ApplyRTLfix(content, 0, true);

            return content;
        }

        private string ApplyRTLfix(string line, int maxCharacters, bool ignoreNumbers)
        {
            bool flag = true;
            bool flag2 = true;
            MatchCollection matchCollection = null;
            if (flag2 || ignoreNumbers)
            {
                Regex regex = new Regex((!ignoreNumbers) ? "<ignoreRTL>(?<val>.*)<\\/ignoreRTL>" : "<ignoreRTL>(?<val>.*)<\\/ignoreRTL>|(?<val>\\d+)");
                matchCollection = regex.Matches(line);
                line = regex.Replace(line, "¬");
            }
            MatchCollection matchCollection2 = null;
            if (flag)
            {
                Regex regex2 = new Regex("(?></?\\w+)(?>(?:[^>'\"]+|'[^']*'|\"[^\"]*\")*)>|\\[.*?\\]");
                matchCollection2 = regex2.Matches(line);
                line = regex2.Replace(line, "¶");
            }
            if (maxCharacters <= 0)
            {
                //line = ArabicFixer.Fix(line);
            }
            else
            {
                Regex regex3 = new Regex(".{0," + maxCharacters + "}(\\s+|$)", RegexOptions.Multiline);
                line = line.Replace("\r\n", "\n");
                line = regex3.Replace(line, "$0\n");
                line = line.Replace("\n\n", "\n");
                string[] array = line.Split(new char[]
                {
                    '\n'
                });
                int i = 0;
                int num = array.Length;
                while (i < num)
                {
                    //array[i] = ArabicFixer.Fix(array[i]);
                    i++;
                }
                line = string.Join("\n", array);
            }
            if (flag && matchCollection != null)
            {
                int count = matchCollection.Count;
                int startIndex = 0;
                for (int j = count - 1; j >= 0; j--)
                {
                    startIndex = line.IndexOf('¬', startIndex);

                    if (startIndex < 0)
                        startIndex = 0;

                    line = line.Remove(startIndex, 1).Insert(startIndex, matchCollection[j].Groups["val"].Value);
                }
            }
            if (flag && matchCollection2 != null)
            {
                int count2 = matchCollection2.Count;
                int startIndex2 = 0;
                for (int k = 0; k < count2; k++)
                {
                    startIndex2 = line.IndexOf('¶', startIndex2);

                    if (startIndex2 < 0)
                        startIndex2 = 0;

                    line = line.Remove(startIndex2, 1).Insert(startIndex2, matchCollection2[k].Value);
                }
            }
            return line;
        }

        private void LoadData(LocalizationData dataCollection)
        {
            _dataCollection = dataCollection;
            //_fontsAsset = _dataCollection.FontsAsset;
            int length = _dataCollection.DataCount;
            _termsDictionary = new Dictionary<string, string[]>(length);
            _dataCollection.FillDicTable(_termsDictionary);
        }

    }
}
