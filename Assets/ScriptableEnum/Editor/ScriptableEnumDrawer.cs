using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using ScriptableEnumSystem.CommonDS;

namespace ScriptableEnumSystem.EditorHandles
{

    [CustomPropertyDrawer(typeof(ScriptableEnum))]
    public class ScriptableEnumDrawer : PropertyDrawer
    {
        public const int NO_SOURCE_ERRCODE = -2;
        static ScriptableEnumsContainer idSources = null;
        GUIContent[] menu;
        string[] idChoices;
        SerializedProperty pathFilterProperty;
        SerializedProperty stringValueProperty;
        SerializedProperty intValueProperty;
        private int lastCachedDrawerVersion;

        private string PrepPropertyName(SerializedProperty property)
        {
            string name = property.name;
            //List<char> narr = name.ToCharArray().ToList();
            //narr[0] = char.ToUpper(narr[0]);
            //for (int i = 1; i < narr.Count; i++)
            //{
            //    if (char.IsUpper(narr[i]))
            //    {
            //        narr.Insert(i, ' ');
            //        i++;
            //    }
            //}

            return ObjectNames.NicifyVariableName(name);
        }


        public override void OnGUI(Rect position, SerializedProperty rootProperty, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, rootProperty);

            if (IsIdSourceNull())
            {
                LoadIdSource();
            }

            if (IsIdSourceNull())
            {
                EditorGUI.LabelField(position, new GUIContent("No Id Source Present,Create or Check Path"));
                return;
            }

            if (!IsIdSouceValid())
            {
                EditorGUI.LabelField(position, new GUIContent("Loaded Id Source is improper, make sure no errors are present"));
                return;
            }

            CacheProperties(rootProperty);

            if (IsMenuCachePending())
            {
                FillChoiceMenu();
                lastCachedDrawerVersion = idSources.DrawerVersion;
            }


            DrawPopup(rootProperty, position, menu, idChoices);

            EditorGUI.EndProperty();
        }



        void DrawPopup(SerializedProperty rootProperty, Rect position, GUIContent[] menu, string[] idChoices)
        {
            GUIContent labelToDisplay = new GUIContent($"{PrepPropertyName(rootProperty)} [{pathFilterProperty.stringValue}]");

            string presentStringIdValue = stringValueProperty.stringValue;
            int oldIndex = GetIndexBasedOf(presentStringIdValue);

            if (oldIndex != NO_SOURCE_ERRCODE)
            {
                int newIndex = EditorGUI.Popup(position, labelToDisplay, oldIndex, menu);
                if (newIndex != -1)
                    SetPresentStringIdValueToTargetProperty(idChoices[newIndex]);
            }
            else
            {
                SetPresentStringIdValueToTargetProperty(idChoices[0]);
            }
        }


        int GetIndexBasedOf(string presentValue)
        {
            int index = Array.IndexOf(idChoices, presentValue);
            if (index >= 0)
                return index;

            Debug.Log($"{presentValue} isnt present in any scriptable container, make sure values are intact, Fixing");
            return NO_SOURCE_ERRCODE;
        }





        void SetPresentStringIdValueToTargetProperty(string value)
        {
            stringValueProperty.stringValue = value;
            intValueProperty.intValue = ScriptableEnum.GetUnsafeId(value);
            //GetIntProperty().intValue = idSources.GetUnsafeId(value);
        }


        void FillChoiceMenu()
        {
            List<SerializedTuple<string, BaseScriptableEnumValueContainer>> systemIdData = idSources.ScriptableContainers;
            List<GUIContent> contentList = new List<GUIContent>();
            List<string> choices = new List<string>();

            for (int i = 0; i < systemIdData.Count; i++)
            {
                AddSystemIdSubMenu(contentList, choices, systemIdData[i]);
            }

            menu = contentList.ToArray();
            idChoices = choices.ToArray();
        }

        void AddSystemIdSubMenu(List<GUIContent> contentList, List<string> choiceList, SerializedTuple<string, BaseScriptableEnumValueContainer> idData)
        {
            string systemId = idData.v1;
            List<string> componentIds = idData.v2.Ids;

            if (!CanAddPath(systemId))
            {
                return;
            }

            for (int i = 0; i < componentIds.Count; i++)
            {
                string choiceId = componentIds[i];
                systemId = ApplyPathFilter(systemId);

                if (string.IsNullOrEmpty(systemId) || string.IsNullOrWhiteSpace(systemId))
                    contentList.Add(new GUIContent($"{choiceId}"));
                else
                    contentList.Add(new GUIContent($"{systemId}/{choiceId}"));

                choiceList.Add(choiceId);
            }

        }

        bool CanAddPath(string originalPath)
        {
            string menuPathFilterValue = pathFilterProperty.stringValue;

            if (string.IsNullOrEmpty(menuPathFilterValue)
                 || string.IsNullOrWhiteSpace(menuPathFilterValue)
                 || originalPath.Contains(ScriptableEnumsContainer.DEFAULT_CONTAINER_MENU_PATH)
                 || originalPath.Contains(ScriptableEnumsContainer.RUNTIME_CONTAINER_MENU_PATH))
                return true;

            return originalPath.Contains(menuPathFilterValue);
        }

        string ApplyPathFilter(string originalPath)
        {
            string menuPathFilterValue = pathFilterProperty.stringValue;

            if (string.IsNullOrEmpty(menuPathFilterValue)
               || string.IsNullOrWhiteSpace(menuPathFilterValue)
               || originalPath.Contains(ScriptableEnumsContainer.DEFAULT_CONTAINER_MENU_PATH)
               || originalPath.Contains(ScriptableEnumsContainer.RUNTIME_CONTAINER_MENU_PATH)
               || originalPath.Length < menuPathFilterValue.Length)
            {
                return originalPath;
            }

            return originalPath.Remove(0, menuPathFilterValue.Length);
        }




        void CacheProperties(SerializedProperty rootProperty)
        {
            if (stringValueProperty == null)
                stringValueProperty = rootProperty.FindPropertyRelative(ScriptableEnum.ValueFieldName);

            if (intValueProperty == null)
                intValueProperty = rootProperty.FindPropertyRelative(ScriptableEnum.intFieldName);

            if (pathFilterProperty == null)
                pathFilterProperty = rootProperty.FindPropertyRelative(ScriptableEnum.menuPathFieldName);
        }

        bool IsIdSourceNull()
        {
            return idSources == null;
        }

        bool IsMenuCachePending()
        {
            return menu == null || menu.Length == 0 || (idSources.DrawerVersion != lastCachedDrawerVersion);
        }

        void LoadIdSource()
        {
            idSources = AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(ASSET_PATHS.ScriptableEnumContainerAssetPath.ResolvedValue);
        }


        private bool IsIdSouceValid()
        {
            return idSources.IsContainerStateValid();

        }
    }
}
