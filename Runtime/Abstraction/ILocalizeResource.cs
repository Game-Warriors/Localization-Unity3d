using GameWarriors.LocalizeDomain.Data;
using System;


namespace GameWarriors.LocalizeDomain.Abstraction
{
    public interface ILocalizeResource
    {
        ILocalizationData LoadResource(ELanguageType languageType);
        void LoadResourceAsync(ELanguageType languageType, Action<ILocalizationData> onLoadDone);
    }
}
