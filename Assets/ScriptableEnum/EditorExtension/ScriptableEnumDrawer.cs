using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
namespace ScriptableEnumSystem.EditorHandles
{
    [CustomPropertyDrawer(typeof(ScriptableEnum))]
    public class ScriptableEnumDrawer : PropertyDrawer
    {
        private const int NO_SOURCE_ERRCODE = -2;
        
        ScriptableEnumsContainer idSources = null;

        private string PrepPropertyName(SerializedProperty property) 
        {
            string name = property.name;
            List<char> narr = name.ToCharArray().ToList();
            narr[0] = char.ToUpper(narr[0]);
            for (int i = 1; i < narr.Count; i++)
            {
                if (char.IsUpper(narr[i]))
                {
                    narr.Insert(i, ' ');
                    i++;
                }
            }

            return new string(narr.ToArray());
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            if (IsIdSourceNull())
                LoadIdSource();

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
           
            GUIContent[] menu;
            string[] idChoices;

//            TryDrawPathFilterLabel();
            FillChoiceMenu();
            DrawPopup(position, menu, idChoices);

            EditorGUI.EndProperty();


            #region FUNCTION_DECLARATIONS

            bool IsIdSourceNull()
            {
                return idSources == null;
            }

            void LoadIdSource()
            {
                idSources = AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(ASSET_PATHS.ScriptableEnumContainerAssetPath);
            }

            //void TryDrawPathFilterLabel()
            //{
            //    GUIContent filterPathLabel = GetFilterPathLabel();
            //    if(filterPathLabel != null) 
            //    {
            //        var r = new Rect(position.position, position.size);
            //        r.height += EditorGUIUtility.singleLineHeight * 2;
            //        EditorGUI.LabelField(r, filterPathLabel);
                    

            //    }
            //}

            void FillChoiceMenu()
            {
                List<PathAssetPair> systemIdData = idSources.ScriptableContainers;
                List<GUIContent> contentList = new List<GUIContent>();
                List<string> choices = new List<string>();

                for (int i = 0; i < systemIdData.Count; i++)
                {
                    AddSystemIdSubMenu(contentList, choices, systemIdData[i]);
                }

                menu = contentList.ToArray();
                idChoices = choices.ToArray();
            }

            void DrawPopup(Rect position, GUIContent[] menu, string[] idChoices)
            {
                GUIContent labelToDisplay = new GUIContent($"{PrepPropertyName(property)} [{GetMenuPathFilterProperty().stringValue}]");

                string presentStringIdValue = GetPresentStringIdValueFromTargetProperty();
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


            void AddSystemIdSubMenu(List<GUIContent> contentList, List<string> choiceList, PathAssetPair idData)
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


            string ApplyPathFilter(string originalPath)
            {
                string menuPathFilterValue = GetMenuPathFilterProperty().stringValue;

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


            bool CanAddPath(string originalPath)
            {
                string menuPathFilterValue = GetMenuPathFilterProperty().stringValue;

                if (string.IsNullOrEmpty(menuPathFilterValue)
                     || string.IsNullOrWhiteSpace(menuPathFilterValue)
                     || originalPath.Contains(ScriptableEnumsContainer.DEFAULT_CONTAINER_MENU_PATH)
                     || originalPath.Contains(ScriptableEnumsContainer.RUNTIME_CONTAINER_MENU_PATH))
                    return true;

                return originalPath.Contains(menuPathFilterValue);
            }


            int GetIndexBasedOf(string presentValue)
            {
                for (int i = 0; i < idChoices.Length; i++)
                {
                    if (idChoices[i] == presentValue)
                    {
                        return i;
                    }
                }

                Debug.Log($"{presentValue} isnt present in any scriptable container, make sure values are intact, Fixing");
                return NO_SOURCE_ERRCODE;
            }


            string GetPresentStringIdValueFromTargetProperty()
            {
                return GetProperty().stringValue;
            }


            void SetPresentStringIdValueToTargetProperty(string value)
            {
                GetProperty().stringValue = value;
                GetIntProperty().intValue = idSources.GetUnsafeId(value);
            }


            GUIContent GetFilterPathLabel()
            {
                var labelFilter = GetMenuPathFilterProperty().stringValue;

                if (string.IsNullOrEmpty(labelFilter)
                 || string.IsNullOrWhiteSpace(labelFilter))
                    return null;

                return new GUIContent(GetMenuPathFilterProperty().stringValue);
            }

            SerializedProperty GetProperty()
            {
                return property.FindPropertyRelative(ScriptableEnum.ValueFieldName);
            }

            SerializedProperty GetIntProperty()
            {
                return property.FindPropertyRelative(ScriptableEnum.intFieldName);
            }

            SerializedProperty GetMenuPathFilterProperty()
            {
                return property.FindPropertyRelative(ScriptableEnum.menuPathFieldName);
            }

         
            #endregion

        }


        private bool IsIdSouceValid()
        {
            return idSources.CheckValidity();

        }
    }
}
