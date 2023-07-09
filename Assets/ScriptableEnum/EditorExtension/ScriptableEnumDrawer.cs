using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ScriptableEnumSystem.EditorHandles
{
    [CustomPropertyDrawer(typeof(ScriptableEnum))]
    public class ScriptableEnumDrawer : PropertyDrawer
    {

        private const string NoneStr = "None";
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
            {
                LoadIdSource();
            }

            if (IsIdSourceNull()) 
            {
                EditorGUI.LabelField(position, new GUIContent("No Id Source Present,Create or Check Path"));
                return;
            }

            GUIContent labelToDisplay = new GUIContent(PrepPropertyName(property));
            GUIContent[] menu;
            string[] idChoices;
             
            FillChoiceMenu();

            string presentStringIdValue = GetPresentStringIdValueFromTargetProperty();
            
            int oldIndex = GetIndexBasedOf(presentStringIdValue); 

            int newIndex = EditorGUI.Popup(position,labelToDisplay, oldIndex,menu);

            if (newIndex != -1)
                SetPresentStringIdValueToTargetProperty(idChoices[newIndex]);//, idChoices[newIndex].objBt);
            

            EditorGUI.EndProperty();


            #region FUNCTION_DECLARATIONS

            bool IsIdSourceNull()
            {
                return idSources == null;
            }
           
            void LoadIdSource()
            {
                idSources = AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(PATHS.ScriptableEnumContainerAssetPath);
            }

            void FillChoiceMenu()
            {
                List<SystemIdsData> systemIdData = idSources.SystemIdsData;
                List<GUIContent> contentList = new List<GUIContent>();
                List<string> choices = new List<string>();

                AddNoneOption(contentList,choices);

                for (int i = 0; i < systemIdData.Count; i++)
                {
                    AddSystemIdSubMenu(contentList, choices,systemIdData[i]);
                }

                menu = contentList.ToArray();
                idChoices = choices.ToArray();
            }

            void AddNoneOption(List<GUIContent> contentList,List<string> actualValuesList)
            {
                string choiceId = NoneStr;
                contentList.Add(new GUIContent(choiceId));
                actualValuesList.Add(ScriptableEnum.NoneS);
            }

            void AddSystemIdSubMenu(List<GUIContent> contentList, List<string> choiceList, SystemIdsData idData)
            {
                string systemId = idData.EnumName;
                List<string> componentIds = idData.Container.Ids;


                for (int i = 0;  i < componentIds.Count; i++)
                {
                    string choiceId = componentIds[i];
                    contentList.Add(new GUIContent($"{systemId}/{choiceId}"));
                    choiceList.Add(choiceId);
                }

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

                return ScriptableEnum.NoneI;
            }

            string GetPresentStringIdValueFromTargetProperty() 
            {
                return GetProperty().stringValue;
            }
            
            void SetPresentStringIdValueToTargetProperty(string value)//,int idVal) 
            {
               GetProperty().stringValue = value;
            }

            SerializedProperty GetProperty() 
            {
                return property.FindPropertyRelative(ScriptableEnum.SerializedValueName);
            }
            #endregion

        }



    }
}
