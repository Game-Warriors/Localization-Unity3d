using System;
using UnityEngine;

namespace GameWarriors.LocalizeDomain.Data
{
    [Serializable]
    public struct TermData : System.IComparable<TermData>
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private string content;

        public readonly string Key => key;
        public readonly string Content => content;

        public TermData(string key, string content)
        {
            this.key = key;
            this.content = content;
        }

        public int CompareTo(TermData other)
        {
            return string.Compare(key, other.key);
        }
    }
}
