using GameWarriors.LocalizeDomain.Data;
using System;


namespace GameWarriors.LocalizeDomain.Abstraction
{
    public interface ILocalizeResource
    {
        void LoadResourceAsync(string assetName, Action<LocalizationData> onLoadDone);
    }
}
