using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GameWarriors.LocalizeDomain.Data;
using GameWarriors.LocalizeDomain.Abstraction;
using GameWarriors.LocalizeDomain.Extensions;
using System.IO;

namespace GameWarriors.LocalizeDomain.Editor
{
    public class LocalizationTextEditor : EditorWindow
    {
        private GUIStyle style = null;
        private GUIStyle style2 = null;
        private GUIStyle styleSelect = null;
        private Vector2 _scrollViewRect;
        private Vector2 v2;

        private string _searchContent;
        private string _selectedKey;
        private Dictionary<string, string[]> _termsTable;
        private Dictionary<string, LocalizationData> _assetTable;

        private int _currentTab = 0;
        private int _lastTab = 0;
        private string[] _languageContents;
        private string[] _tabs = new string[] { "Terms", "New Term", "Language", "Font Assets" };
        private string[] _languageNames;

        private int LanguageCount => _languageContents.Length - 1;
        public int TermCount => _termsTable?.Keys.Count ?? 0;

        [MenuItem("Tools/Localization/Open Editor")]
        static void ShowLocalizationWindow()
        {
            LocalizationTextEditor localizationEditor = (LocalizationTextEditor)GetWindow(typeof(LocalizationTextEditor));
            localizationEditor.Show();
            localizationEditor.Initialization();
        }

        private void Initialization()
        {
            _searchContent = string.Empty;
            int languageCount = Enum.GetNames(typeof(ELanguageType)).Length - 1;
            _languageContents = new string[languageCount + 1];
            _languageNames = new string[languageCount];

            for (int i = 0; i < languageCount; ++i)
            {
                _languageNames[i] = ((ELanguageType)i).ToString();
            }
            _selectedKey = string.Empty;
            Directory.CreateDirectory("Assets/AssetData/Resources");

            LoadAllLocalizeData();

            RefreshPrefab();
            RefreshContentList();

            style = new GUIStyle();
            style.font = (Font)Resources.Load("Fonts/Farnaz5");
            Texture2D _img = (Texture2D)Resources.Load("bg1");
            Texture2D _img2 = (Texture2D)Resources.Load("bg2");
            Texture2D _img3 = (Texture2D)Resources.Load("bg3");
            style.active.background = _img3;
            style.normal.background = _img2;
            style.active.textColor = Color.red; // not working
            style.hover.textColor = Color.blue; // not working
            style.normal.textColor = Color.black;

            int border = 30;

            style.border.left = border; // not working, since backgrounds aren't showing
            style.border.right = border; // ---
            style.border.top = border; // ---
            style.border.bottom = border; // ---
            style.stretchWidth = true; // ---
            style.stretchHeight = true; // not working, since backgrounds aren't showing
            style.alignment = TextAnchor.MiddleLeft;

            style2 = new GUIStyle();
            style2.font = (Font)Resources.Load("Fonts/Farnaz5");
            style2.active.background = _img3;
            style2.normal.background = _img;
            style2.active.textColor = Color.red; // not working
            style2.hover.textColor = Color.blue; // not working
            style2.normal.textColor = Color.black;
            style2.border.left = border; // not working, since backgrounds aren't showing
            style2.border.right = border; // ---
            style2.border.top = border; // ---
            style2.border.bottom = border; // ---
            style2.stretchWidth = true; // ---
            style2.stretchHeight = true; // not working, since backgrounds aren't showing
            style2.alignment = TextAnchor.MiddleLeft;


            styleSelect = new GUIStyle();
            styleSelect.font = (Font)Resources.Load("Fonts/Farnaz5");

            styleSelect.active.background = _img3;
            styleSelect.normal.background = _img3;
            styleSelect.active.textColor = Color.red; // not working
            styleSelect.hover.textColor = Color.black; // not working
            styleSelect.normal.textColor = Color.white;
            styleSelect.border.left = border; // not working, since backgrounds aren't showing
            styleSelect.border.right = border; // ---
            styleSelect.border.top = border; // ---
            styleSelect.border.bottom = border; // ---
            styleSelect.stretchWidth = true; // ---
            styleSelect.stretchHeight = true; // not working, since backgrounds aren't showing
            styleSelect.alignment = TextAnchor.MiddleLeft;
        }

        private void LoadAllLocalizeData()
        {
            int length = LanguageCount;
            _termsTable = new Dictionary<string, string[]>(400);
            _assetTable = new Dictionary<string, LocalizationData>(length);
            for (int i = 0; i < length; ++i)
            {
                string languageKey = _languageNames[i];
                string assetPath = string.Format(LocalizationData.ASSET_DATA_PATH, languageKey);
                LocalizationData localizeData = AssetDatabase.LoadAssetAtPath<LocalizationData>(assetPath);

                if (localizeData == null)
                {
                    localizeData = CreateInstance<LocalizationData>();
                    AssetDatabase.CreateAsset(localizeData, assetPath);
                }
                _assetTable.Add(languageKey, localizeData);

                FillTermTable(i, length, localizeData);
            }
        }

        private void FillTermTable(int languageId, int languageCount, LocalizationData localizationData)
        {
            foreach ((string key, string value) item in localizationData.AllItems)
            {
                if (_termsTable.ContainsKey(item.key))
                {
                    _termsTable[item.key][languageId] = item.value;
                }
                else
                {
                    string[] items = new string[languageCount];
                    items[languageId] = item.value;
                    _termsTable.Add(item.key, items);
                }
            }
        }

        void RefreshPrefab()
        {
            _searchContent = string.Empty;
        }

        public bool FindString(string a, string b)
        {
            return a.IndexOf(b) > -1;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            _currentTab = GUILayout.Toolbar(_currentTab, _tabs);
            switch (_currentTab)
            {
                case 0:
                    DrawTermsListTab();
                    break;
                case 1:
                    DrawNewTermTab();
                    break;
                case 3:
                    DrawFontAssetTab();
                    break;

            }
            _lastTab = _currentTab;
        }

        private void DrawNewTermTab()
        {
            int count = LanguageCount + 1;
            if (_lastTab != _currentTab)
            {
                Array.Clear(_languageContents, 0, count);
            }

            SetLocalizeDataDirty();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Term :", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < count; i++)
            {
                GUILayout.BeginHorizontal();
                if (i == 0)
                    GUILayout.Label("key : ", GUILayout.Width(60));
                else
                    GUILayout.Label(_languageNames[i - 1] + " : ", GUILayout.Width(60));

                _languageContents[i] = EditorGUILayout.TextField(_languageContents[i], GUILayout.Width(700));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New Term", GUILayout.Width(200), GUILayout.Height(40)))
            {
                Array.Clear(_languageContents, 0, count);
            }
            GUILayout.Space(50);
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(40)) && LanguageCount > 0)
            {
                bool HaveTerm = _termsTable.ContainsKey(_languageContents[0]);
                if (HaveTerm)
                {
                    EditorUtility.DisplayDialog("No Save", "Exists Term in Dictionary", "Ok");
                }
                else
                {
                    int b = EditorUtility.DisplayDialogComplex("Do Saved", "Are You Sure?", "Save", "cancle", "Dont Save");
                    if (b == 0)
                    {
                        SetLocalizeDataDirty();
                        string[] contexts = new string[LanguageCount];
                        Term term = new Term(_languageContents[0], contexts);
                        for (int j = 0; j < LanguageCount; ++j)
                        {
                            term.Contents[j] = _languageContents[j + 1];
                        }
                        AddData(term);
                        Array.Clear(_languageContents, 0, LanguageCount);
                        RefreshPrefab();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSearchSection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("search :", GUILayout.Width(50));
            _searchContent = EditorGUILayout.TextField(_searchContent, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        private void DrawModifySection()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(45)))
            {
                int b = EditorUtility.DisplayDialogComplex("Save Term", "Are You Sure?", "Save", "Cancle", "Don't Save");
                if (b == 0 && LanguageCount > 0)
                {
                    SetLocalizeDataDirty();
                    //RefreshPrefab();
                    Term t = new Term(_languageContents[0], new string[LanguageCount]);

                    for (int i = 0; i < LanguageCount; i++)
                    {
                        t.Contents[i] = _languageContents[i + 1];
                    }
                    UpdateData(t);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log("save");
                }
                //}
            }
            GUILayout.Space(20);
            if (_termsTable == null)
                Initialization();
            string key = _selectedKey;
            if (!string.IsNullOrEmpty(key))
            {
                GUILayout.Label(_termsTable.Translate(key, (ELanguageType)1), GUILayout.Width(500));
                GUILayout.Space(80);
                if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(45)))
                {
                    if (EditorUtility.DisplayDialogComplex("Do Delete", "Are You Sure For Delete?", "Delete", "cancle", "Dont Delete") == 0)
                    {
                        SetLocalizeDataDirty();
                        RemoveData(key);
                        RefreshPrefab();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        _selectedKey = string.Empty;
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawTermsListTab()
        {
            EditorGUILayout.SelectableLabel("Current Language");

            DrawSearchSection();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            int dataCount = TermCount;
            GUILayout.Label("count Terms :" + dataCount, GUILayout.Width(150));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            //GUILayout.Label("_____________________________________________________________", GUILayout.Width(480));
            if (dataCount == 0)
            {
                EditorGUILayout.EndVertical();
                return;
            }
            _scrollViewRect = EditorGUILayout.BeginScrollView(_scrollViewRect, GUILayout.Height(500), GUILayout.Width(420));
            int counter = 0;
            foreach (var item in _termsTable.Keys)
            {
                if (string.IsNullOrEmpty(_searchContent))
                {
                    if (_selectedKey == item)
                    {
                        if (GUILayout.Button("      " + item, styleSelect, GUILayout.Width(400), GUILayout.Height(50)))
                        {
                            _selectedKey = item;
                            RefreshContentList();
                        }
                    }
                    else if (GUILayout.Button("      " + item, counter % 2 == 0 ? style : style2, GUILayout.Width(400), GUILayout.Height(35)))
                    {
                        _selectedKey = item;
                        RefreshContentList();
                    }
                }
                else
                {
                    if (_termsTable == null)
                        Initialization();

                    string content = _termsTable.Translate(item, (ELanguageType)1);

                    if (FindString(item.ToLower(), _searchContent.ToLower()) || FindString(content.ToLower(), _searchContent.ToLower()))
                    {
                        if (_selectedKey == item)
                        {
                            if (GUILayout.Button("      " + item, styleSelect, GUILayout.Width(400), GUILayout.Height(60)))
                            {
                                _selectedKey = item;
                                RefreshContentList();
                            }
                        }
                        else if (GUILayout.Button("      " + item, counter % 2 == 0 ? style : style2, GUILayout.Width(400), GUILayout.Height(45)))
                        {
                            _selectedKey = item;
                            RefreshContentList();
                        }
                    }

                }
                ++counter;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Space(10);
            v2 = EditorGUILayout.BeginScrollView(v2, GUILayout.Width(520), GUILayout.Height(300));
            if (_lastTab != _currentTab)
            {
                RefreshContentList();
            }
            ShowTranslateTerm();
            EditorGUILayout.EndScrollView();
            GUILayout.Space(100);
            DrawModifySection();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawFontAssetTab()
        {
            int length = LanguageCount;
            //if (_localizeData.FontsAsset.Length < _languageCount)
            //{
            //    TMP_FontAsset[] fonts = _localizeData.FontsAsset;
            //    Array.Resize(ref fonts, _languageCount);
            //    _localizeData.FontsAsset = fonts;
            //}
            //for (int i = 0; i < length; ++i)
            //{
            //    string lable = ((ELanguageType)i).ToString();
            //    _localizeData.FontsAsset[i] = EditorGUILayout.ObjectField(lable + " Font Asset", _localizeData.FontsAsset[i], typeof(TMP_FontAsset), false) as TMP_FontAsset;
            //}
            //if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(40)))
            //{
            //    EditorUtility.SetDirty(_localizeData);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();
            //}
        }


        private void RefreshContentList()
        {
            int length = LanguageCount + 1;
            Array.Clear(_languageContents, 0, length);
            string key = _selectedKey;
            if (LanguageCount > 0)
                _languageContents[0] = key;
            if (_termsTable == null)
                Initialization();

            if (!string.IsNullOrEmpty(key))
            {
                for (int i = 1; i < length; ++i)
                {
                    _languageContents[i] = _termsTable.Translate(key, (ELanguageType)i - 1);
                }
            }
        }

        public void ShowTranslateTerm()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Term : ", GUILayout.Width(70));
            // I2Languages.mTerms[index].Term= EditorGUILayout.TextField(I2Languages.mTerms[index].Term, GUILayout.Width(700));
            if (LanguageCount > 0)
                _languageContents[0] = EditorGUILayout.TextField(_languageContents[0], GUILayout.Width(700));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Space(10);

            int length = LanguageCount + 1;
            for (int i = 1; i < length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Enum.GetNames(typeof(ELanguageType))[i - 1] + " : ", GUILayout.Width(70));
                _languageContents[i] = EditorGUILayout.TextField(_languageContents[i], GUILayout.Width(700));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }

        private void RefreshListTerms()
        {
            int termCount = TermCount;
            if (termCount == 0)
                return;
            EditorUtility.DisplayProgressBar("Please Wait ....", "For Update List Term", 0.001f);
            float f = 1f / termCount;
            for (var i = 0; i < termCount; ++i)
            {
                EditorUtility.DisplayProgressBar("Please Wait ....", "For Update List Term", f * i);
            }
            EditorUtility.ClearProgressBar();
        }

        private void AddData(Term newTerm)
        {
            if (!_termsTable.ContainsKey(newTerm.Key))
            {
                _termsTable.Add(newTerm.Key, newTerm.Contents);
                int counter = 0;
                foreach (var item in _assetTable.Values)
                {
                    LocalizationData localizeData = item;
                    string word = newTerm.Contents[counter];
                    if (!string.IsNullOrEmpty(word))
                    {
                        localizeData.AddTerm(newTerm.Key, word);
                        EditorUtility.SetDirty(localizeData);
                    }
                    ++counter;
                }
            }
        }

        private void RemoveData(string key)
        {
            if (_termsTable.ContainsKey(key))
            {
                _termsTable.Remove(key);
                int counter = 0;
                foreach (var item in _assetTable.Values)
                {
                    LocalizationData localizeData = item;
                    localizeData.RemoveTerm(key);
                    EditorUtility.SetDirty(localizeData);
                    ++counter;
                }
            }
        }

        private void UpdateData(Term targetTerm)
        {
            if (_termsTable.ContainsKey(targetTerm.Key))
            {
                _termsTable[targetTerm.Key] = targetTerm.Contents;
                int counter = 0;
                foreach (var item in _assetTable.Values)
                {
                    LocalizationData localizeData = item;
                    string word = targetTerm.Contents[counter];
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (localizeData.UpdateTerm(targetTerm.Key, word))
                            EditorUtility.SetDirty(localizeData);
                    }
                    ++counter;
                }
            }
        }

        private void SetLocalizeDataDirty()
        {
            foreach (var localizeData in _assetTable.Values)
            {
                EditorUtility.SetDirty(localizeData);
            }
        }
    }
}