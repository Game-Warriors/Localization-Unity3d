using GameWarriors.LocalizeDomain.Abstraction;
using System.Collections.Generic;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Extensions
{
    public static class LocalizationExtension
    {
        public static string Translate(this Dictionary<string, string[]> termsDictionary, string key, ELanguageType language)
        {
            if (string.IsNullOrEmpty(key))
            {
                //#if DEVELOPMENT
                //                Debug.LogWarning($"Localization -> key is empty or null!");
                //#endif
                return string.Empty;
            }

            if (termsDictionary == null)
                return default;

            if (termsDictionary.TryGetValue(key, out string[] items))
            {
                if ((int) language < items?.Length)
                {
                    return items[(int) language];
                }

                Debug.LogWarning($"Localization -> key:{key} has not content for language:{language}!");
                return key;
            }
            else
            {
                Debug.LogWarning($"Localization -> key:{key} was not found!");
                return key;
            }
        }
    }
}