using GameWarriors.LocalizeDomain.Abstraction;
using GameWarriors.LocalizeDomain.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Editor
{
    public class ConvertCSVToLocalizeData
    {
        private const string SPLIT_STRING = ";";

        [MenuItem("Tools/Localization/Convert CSV To LocalizationData")]
        private static void ConvertCSVToLocalizationData()
        {
            string[] languageNames = Enum.GetNames(typeof(ELanguageType));
            int languageCount = languageNames.Length;
            for (int i = 1; i < languageCount; ++i)
            {
                LocalizationData localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(string.Format(LocalizationData.ASSET_DATA_PATH, languageNames[i]));
                List<string[]> languageData = LoadCSVToLocalizationData($"Assets/AssetData/LocalizationData/LocalizationEn{languageNames[i]}");
                localizationData.RefillData(FillLocalizeData(languageData));
                EditorUtility.SetDirty(localizationData);
            }
        }

        private static List<string[]> LoadCSVToLocalizationData(string path)
        {
            List<string[]> data = new List<string[]>();
            TextAsset csv = Resources.Load(path) as TextAsset;
            StringReader reader = new StringReader(csv.text);
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                string[] items = line.Split(SPLIT_STRING.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                data.Add(items);
            }
            return data;
        }

        private static IEnumerable<(string key, string value)> FillLocalizeData(List<string[]> languageData)
        {
            int length = languageData.Count;
            for (int i = 0; i < length; ++i)
            {
                yield return (languageData[i][0], languageData[i][1]);
            }
        }
    }

}

