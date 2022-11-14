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
        private int _indexSelect = 0;
        private Dictionary<string, string[]> _termsTable;
        private LocalizationData _localizeData;
        private int _languageCount;
        private int _currentTab = 0;
        private int _lastTab = 0;
        private string[] _languageContents;
        private string[] _tabs = new string[] { "Terms", "New Term", "Language", "Font Assets" };
        private string[] _languageNames;

        [MenuItem("Tools/Localization Editor")]
        static void ShowLocalizationWindow()
        {
            LocalizationTextEditor localizationEditor = (LocalizationTextEditor)GetWindow(typeof(LocalizationTextEditor));
            localizationEditor.Show();
            localizationEditor.Initialization();
        }

        private void Initialization()
        {
            _searchContent = string.Empty;
            _languageCount = Enum.GetValues(typeof(ELanguageType)).Length - 1;
            _languageContents = new string[_languageCount + 1];
            _languageNames = new string[_languageCount];

            for (int i = 0; i < _languageCount; ++i)
            {
                _languageNames[i] = ((ELanguageType)i).ToString();
            }
            _indexSelect = 0;
            Directory.CreateDirectory("Assets/AssetData/Resources");
            _localizeData = AssetDatabase.LoadAssetAtPath<LocalizationData>(LocalizationData.ASSET_DATA_PATH);
            if (_localizeData == null)
            {
                _localizeData = CreateInstance<LocalizationData>();
                AssetDatabase.CreateAsset(_localizeData, LocalizationData.ASSET_DATA_PATH);
            }
            _termsTable = new Dictionary<string, string[]>(400);
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

        void RefreshPrefab()
        {
            _searchContent = string.Empty;
            _localizeData.FillDicTable(_termsTable);
        }

        public bool Findstring(string a, string b)
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
            int count = _languageCount + 1;
            if (_lastTab != _currentTab)
            {
                Array.Clear(_languageContents, 0, count);
            }

            EditorUtility.SetDirty(_localizeData);
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
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(40)) && _languageCount > 0)
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
                        EditorUtility.SetDirty(_localizeData);
                        string[] contexts = new string[_languageCount];
                        Term term = new Term(_languageContents[0], contexts);
                        for (int j = 0; j < _languageCount; ++j)
                        {
                            term.Contents[j] = _languageContents[j + 1];
                        }
                        AddData(term);
                        Array.Clear(_languageContents, 0, _languageCount);
                        RefreshPrefab();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawTermsListTab()
        {
            EditorGUILayout.SelectableLabel("Current Language");
            GUILayout.BeginHorizontal();
            GUILayout.Label("search :", GUILayout.Width(50));
            _searchContent = EditorGUILayout.TextField(_searchContent, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("count Terms :" + _localizeData.DataCount, GUILayout.Width(150));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            //GUILayout.Label("_____________________________________________________________", GUILayout.Width(480));
            if (_localizeData.DataCount == 0)
                return;
            _scrollViewRect = EditorGUILayout.BeginScrollView(_scrollViewRect, GUILayout.Height(500), GUILayout.Width(420));
            for (int i = 0; i < _localizeData.DataCount; ++i)
            {
                if (string.IsNullOrEmpty(_searchContent))
                {
                    if (_indexSelect == i)
                    {
                        if (GUILayout.Button("      " + _localizeData.GetTermKeyAt(i), styleSelect, GUILayout.Width(400), GUILayout.Height(50)))
                        {
                            _indexSelect = i;
                            RefreshContentList();
                        }
                    }
                    else
                    if (GUILayout.Button("      " + _localizeData.GetTermKeyAt(i), i % 2 == 0 ? style : style2, GUILayout.Width(400), GUILayout.Height(35)))
                    {
                        _indexSelect = i;
                        RefreshContentList();
                    }
                }
                else
                {
                    if (_termsTable == null)
                        Initialization();

                    string content = _termsTable.Translate(_localizeData.GetTermKeyAt(i), (ELanguageType)1);

                    if (Findstring(_localizeData.GetTermKeyAt(i).ToLower(), _searchContent.ToLower()) || Findstring(content.ToLower(), _searchContent.ToLower()))
                    {
                        if (_indexSelect == i)
                        {
                            if (GUILayout.Button("      " + _localizeData.GetTermKeyAt(i), styleSelect, GUILayout.Width(400), GUILayout.Height(60)))
                            {
                                _indexSelect = i;
                                RefreshContentList();
                            }
                        }
                        else
                        if (GUILayout.Button("      " + _localizeData.GetTermKeyAt(i), i % 2 == 0 ? style : style2, GUILayout.Width(400), GUILayout.Height(45)))
                        {
                            _indexSelect = i;
                            RefreshContentList();
                        }
                    }

                }
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
            ShowTranslateterm(_indexSelect);
            EditorGUILayout.EndScrollView();
            GUILayout.Space(100);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(45)))
            {
                int b = EditorUtility.DisplayDialogComplex("Save Term", "Are You Sure?", "Save", "Cancle", "Don't Save");
                if (b == 0 && _languageCount > 0)
                {
                    EditorUtility.SetDirty(_localizeData);
                    //RefreshPrefab();
                    Term t = new Term(_languageContents[0], new string[_languageCount]);

                    for (int i = 0; i < _languageCount; i++)
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
            GUILayout.Label(_termsTable.Translate(_localizeData.GetTermKeyAt(_indexSelect), (ELanguageType)1), GUILayout.Width(500));
            GUILayout.Space(80);
            if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(45)))
            {
                if (EditorUtility.DisplayDialogComplex("Do Delete", "Are You Sure For Delete?", "Delete", "cancle", "Dont Delete") == 0)
                {
                    EditorUtility.SetDirty(_localizeData);
                    RemoveData(_localizeData.GetTermKeyAt(_indexSelect));
                    RefreshPrefab();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    if (_localizeData.DataCount <= _indexSelect)
                        _indexSelect = _localizeData.DataCount - 1;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawFontAssetTab()
        {
            int length = _languageCount;
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
            int length = _languageCount + 1;
            Array.Clear(_languageContents, 0, length);
            if (_languageCount > 0)
                _languageContents[0] = _localizeData.GetTermKeyAt(_indexSelect);
            if (_termsTable == null)
                Initialization();

            for (int i = 1; i < length; ++i)
            {
                _languageContents[i] = _termsTable.Translate(_localizeData.GetTermKeyAt(_indexSelect), (ELanguageType)i - 1);
            }
        }

        public void ShowTranslateterm(int index)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Term : ", GUILayout.Width(70));
            // I2Languages.mTerms[index].Term= EditorGUILayout.TextField(I2Languages.mTerms[index].Term, GUILayout.Width(700));
            if (_languageCount > 0)
                _languageContents[0] = EditorGUILayout.TextField(_languageContents[0], GUILayout.Width(700));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Space(10);

            int length = _languageCount + 1;
            for (int i = 1; i < length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Enum.GetNames(typeof(ELanguageType))[i-1] + " : ", GUILayout.Width(70));
                _languageContents[i] = EditorGUILayout.TextField(_languageContents[i], GUILayout.Width(700));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }

        private void RefreshListTerms()
        {
            if (_localizeData.DataCount == 0)
                return;
            EditorUtility.DisplayProgressBar("Please Wait ....", "For Update List Term", 0.001f);
            float f = 1f / _localizeData.DataCount;
            for (var i = 0; i < _localizeData.DataCount; ++i)
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
                _localizeData.AddTerm(newTerm);
            }
        }

        private void RemoveData(string key)
        {
            if (_termsTable.ContainsKey(key))
            {
                _termsTable.Remove(key);
                _localizeData.RemoveTerm(key);
            }
        }

        private void UpdateData(Term targetTerm)
        {
            if (_termsTable.ContainsKey(targetTerm.Key))
            {
                _termsTable[targetTerm.Key] = targetTerm.Contents;
                _localizeData.UpdateTerm(targetTerm);
            }
        }

    }
}