using System;
using System.Collections.Generic;

namespace GameWarriors.LocalizeDomain.Abstraction
{
    public interface ILocalizationData : IDisposable
    {
        int DataCount { get; }

        void FillDicTable(IDictionary<string, string> termsDictionary);
    }
}