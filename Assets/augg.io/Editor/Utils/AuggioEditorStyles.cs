#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.Editor.Validation;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    [Serializable]
    internal class AuggioEditorStyles
    {
        [SerializeField] private GUIStyle _normalText;
        [SerializeField] private GUIStyle _boldText;
        [SerializeField] private GUIStyle _h1;
        [SerializeField] private GUIStyle _h2;
        [SerializeField] private GUIStyle _h3;
        [SerializeField] private GUIStyle _textField;
        [SerializeField] private GUIStyle _primaryButton;
        [SerializeField] private GUIStyle _dropDown;
        [SerializeField] private GUIStyle _experienceButtonSkinStyle;
        [SerializeField] private GUIStyle tableRowStyle;
        [SerializeField] private GUIStyle tableHeaderStyle;
        [SerializeField] private GUIStyle _loadingLabelStyle;

        [SerializeField] private Color _green;
        [SerializeField] private Color _red;

        [SerializeField] private Font _font;

        [SerializeField] private bool initialized;
        
        private static AuggioEditorStyles _instance;
        
        

        internal static AuggioEditorStyles Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuggioEditorStyles();
                }
                return _instance;
            }
        }

        internal void Initialize(EditorWindow rootWindow)
        {
            try
            {
                string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(rootWindow));
                string spriteFolderPath = scriptPath.Substring(0, scriptPath.LastIndexOf('/') + 1);
                string fontPath = spriteFolderPath + "LiberationSans.ttf";
                _font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);

                _normalText = new GUIStyle(EditorStyles.label);
                _normalText.font = _font;

                _boldText = new GUIStyle(EditorStyles.boldLabel);
                _boldText.font = _font;

                _h1 = new GUIStyle(EditorStyles.label);
                _h1.font = _font;
                _h1.fontSize = 32;
                _h1.fontStyle = FontStyle.Bold;

                _h2 = new GUIStyle(EditorStyles.label);
                _h2.font = _font;
                _h2.fontSize = 26;
                _h2.fontStyle = FontStyle.Normal;

                _h3 = new GUIStyle(EditorStyles.label);
                _h3.font = _font;
                _h3.fontSize = 20;
                _h3.fontStyle = FontStyle.Normal;

                _textField = new GUIStyle(EditorStyles.textField);
                _textField.font = _font;

                _primaryButton = new GUIStyle(EditorStyles.miniButton);
                _primaryButton.fixedHeight = 32;
                //_primaryButton.normal.background = GetColorTexture(new Color(76/255f, 0/255f, 172/255f));
                //_primaryButton.hover.background = GetColorTexture(new Color(94/255f, 2/255f, 209/255f));
                _primaryButton.border = new RectOffset(10, 10, 10, 10);
                _primaryButton.font = _font;

                _dropDown = new GUIStyle(EditorStyles.popup);
                _dropDown.fixedHeight = 32;
                _dropDown.border = new RectOffset(10, 10, 10, 10);
                _dropDown.font = _font;

                _experienceButtonSkinStyle =
                    new GUIStyle(EditorStyles
                        .miniButton);
                _experienceButtonSkinStyle.font = _font;
                _experienceButtonSkinStyle.fixedHeight = 64;
                _experienceButtonSkinStyle.alignment = TextAnchor.MiddleLeft;
                _experienceButtonSkinStyle.fontSize = 24;
                _experienceButtonSkinStyle.padding = new RectOffset(32, 32, 16, 16);

                tableRowStyle = CreatAnchorDataSkinStyle(GUIStyle.none, new Color(0.35f, 0.35f, 0.35f), 0);
                tableRowStyle.fixedHeight = 48;

                tableHeaderStyle = CreatAnchorDataSkinStyle(GUIStyle.none, new Color(0.25f, 0.25f, 0.25f), 0);
                tableHeaderStyle.fixedHeight = 48;

                _loadingLabelStyle = new GUIStyle(EditorStyles.label);
                _loadingLabelStyle.alignment = TextAnchor.MiddleCenter;
                _loadingLabelStyle.font = _font;
                _loadingLabelStyle.fontSize = 16;

                _green = new Color(0f, 0.8f, 0f);
                _red = new Color(0.6f, 0f, 0f);
                initialized = true;
            }
            catch (Exception)
            {
                initialized = false;
            }
        }

        internal static GUIStyle WithTextColor(GUIStyle guiStyle, Color color)
        {
            GUIStyle centeredStyle = new GUIStyle(guiStyle);
            centeredStyle.normal.textColor = color;
            return centeredStyle;
        }

        
        internal static GUIStyle WithTextAlignment(GUIStyle guiStyle, TextAnchor anchor)
        {
            GUIStyle centeredStyle = new GUIStyle(guiStyle);
            centeredStyle.alignment = anchor;
            return centeredStyle;
        }

        internal static GUIStyle WithMargin(GUIStyle guiStyle, RectOffset margin)
        {
            GUIStyle marginStyle = new GUIStyle(guiStyle);
            marginStyle.margin = margin;
            return marginStyle;
        }
        
        internal static void DrawHelpBoxWithButtons(string message, MessageType type, List<ErrorButton> buttons)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox(message, type);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (ErrorButton button in buttons)
            {
                if (GUILayout.Button(button.ButtonText, GUILayout.Width(200)))
                {
                    button.OnClick?.Invoke();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private GUIStyle CreatAnchorDataSkinStyle(GUIStyle parentGUIStyle, Color backgroundColor = default, int padding = 20)
        {
            GUIStyle customBoxSkin = new GUIStyle(parentGUIStyle)
            {
                padding = new RectOffset(padding, padding, padding, padding)
            };
        
            if (backgroundColor != default)
            {
                customBoxSkin.normal.background = GetColorTexture(backgroundColor);
            }
        
            return customBoxSkin;
        }

        private Texture2D GetColorTexture(Color color)
        {
            Color[] pix = new Color[2 * 2];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(2, 2);
            backgroundTexture.SetPixels(pix);
            backgroundTexture.Apply();

            return backgroundTexture;
        }

        internal bool Initialized => initialized;

        internal GUIStyle H1 => _h1;

        internal GUIStyle H2 => _h2;

        internal GUIStyle H3 => _h3;

        internal GUIStyle NormalText => _normalText;

        internal GUIStyle BoldText => _boldText;

        internal GUIStyle TextField => _textField;

        internal GUIStyle PrimaryButton => _primaryButton;

        internal GUIStyle DropDown => _dropDown;

        internal GUIStyle ExperienceButtonSkinStyle => _experienceButtonSkinStyle;

        internal GUIStyle TableRowStyle => tableRowStyle;

        internal GUIStyle TableHeaderStyle => tableHeaderStyle;

        internal GUIStyle LoadingLabelStyle => _loadingLabelStyle;

        internal Color Green => _green;

        internal Color Red => _red;
    }
}
#endif