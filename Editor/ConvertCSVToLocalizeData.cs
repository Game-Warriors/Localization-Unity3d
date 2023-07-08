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
        private const string SPLIT_STRING = ",";

        [MenuItem("Tools/Localization/Convert CSV To LocalizationData")]
        private static void ConvertCSVToLocalizationData()
        {
            string[] languageNames = Enum.GetNames(typeof(ELanguageType));
            int languageCount = languageNames.Length;
            for (int i = 1; i < languageCount; ++i)
            {
                string assetPath = string.Format(LocalizationData.ASSET_DATA_PATH, languageNames[i]);
                LocalizationData localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(assetPath);
                List<string[]> languageData = LoadCSVToLocalizationData($"Assets/AssetData/LocalizationData/Localization{languageNames[i]}.csv");
                if (languageData != null)
                {
                    if (localizationData == null)
                    {
                        localizationData = ScriptableObject.CreateInstance<LocalizationData>();
                        AssetDatabase.CreateAsset(localizationData, assetPath);
                    }
                    localizationData.RefillData(FillLocalizeData(languageData));
                    EditorUtility.SetDirty(localizationData);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static List<string[]> LoadCSVToLocalizationData(string path)
        {
            TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (csv != null)
            {
                List<string[]> data = new List<string[]>();
                StringReader reader = new StringReader(csv.text);
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    string[] items = line.Split(SPLIT_STRING.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    data.Add(items);
                }
                return data;
            }
            return null;
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

