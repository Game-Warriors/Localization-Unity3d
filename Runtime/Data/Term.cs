using UnityEngine;

namespace GameWarriors.LocalizeDomain.Data
{
    [System.Serializable]
    public struct Term : System.IComparable<Term>
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private string[] contents;

        public string Key => key;
        public string [] Contents => contents;

        public Term(string key, string[] contents)
        {
            this.key = key;
            this.contents = contents;
        }

        public int CompareTo(Term other)
        {
            return string.Compare(key, other.key);
        }
    }
}
