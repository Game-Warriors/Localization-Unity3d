using System;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Abstraction
{
    public enum ELanguageType
    {
        None = -1,        
        Fa = 0,
        En = 1,
        Fr = 2,
        Ge = 3,
        Tu = 4,
        Ar = 5
    }

    public interface ILocalize
    {
        event Action OnLanguageChanged;
        bool IsRTL { get; }
        ELanguageType CurrentLanguage { get; }
        string GetTermTranslation(string term);
        string GetTermTranslation(string term, ELanguageType languageType);
        Sprite GetLocalizedSprite(string key);
        //TMP_FontAsset GetFontAsset(ELanguageType languageType);
        void SetLanguage(ELanguageType languageType);
    }
}