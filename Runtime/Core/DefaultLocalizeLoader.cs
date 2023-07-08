using GameWarriors.LocalizeDomain.Abstraction;
using GameWarriors.LocalizeDomain.Data;
using System;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Core
{
    public class DefaultLocalizeLoader : ILocalizeResource
    {
        public void LoadResourceAsync(ELanguageType languageType, Action<ILocalizationData> onLoadDone)
        {
            string dataPath = LocalizationData.GetAssetPath(languageType);
            ResourceRequest operation = Resources.LoadAsync<LocalizationData>(dataPath);
            operation.completed += (asyncOperation) => onLoadDone((asyncOperation as ResourceRequest).asset as LocalizationData);
        }

        public ILocalizationData LoadResource(ELanguageType languageType)
        {
            string dataPath = LocalizationData.GetAssetPath(languageType);
            return Resources.Load<LocalizationData>(dataPath);
        }
    }
}