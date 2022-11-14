using GameWarriors.LocalizeDomain.Abstraction;
using GameWarriors.LocalizeDomain.Data;
using System;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Core
{
    public class DefaultLocalizeLoader : ILocalizeResource
    {
        public void LoadResourceAsync(string assetName, Action<LocalizationData> onLoadDone)
        {
            ResourceRequest operation = Resources.LoadAsync<LocalizationData>(assetName);
            operation.completed += (asyncOperation) => onLoadDone((asyncOperation as ResourceRequest).asset as LocalizationData);
        }
    }
}