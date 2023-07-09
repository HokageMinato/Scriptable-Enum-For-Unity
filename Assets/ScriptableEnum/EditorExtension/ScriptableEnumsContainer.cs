using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ScriptableEnumSystem.EditorHandles
{
    public class ScriptableEnumsContainer : ScriptableObject
    {
        
        [SerializeField] private List<SystemIdsData> idsData;
        public List<SystemIdsData> SystemIdsData
        {
            get
            {
                return idsData;
            }
        }

        [ContextMenu("Check for Duplicates")]
        public void SearchForDuplicates()
        {
            SearchForDuplicateScriptables();
            SearchForDuplicateIds();
        }


        public void AssignId()
        {
            for (int i = 0; i < idsData.Count; i++)
            {
                var data = idsData[i];
                if (data.Container == null)
                    return;

                var idSRCData = data.Container.Ids;

                for (int j = 0; j < idSRCData.Count; j++)
                {
                    var strId = idSRCData[j];
                    strId = strId.Trim();

                    idSRCData[j]= strId;
                }


                data.SetDirty();
            }

        }



       

        private void SearchForDuplicateScriptables() 
        {
            HashSet<ScriptableEnumValueContainer> container = new HashSet<ScriptableEnumValueContainer>();
            for (int i = 0; i < idsData.Count;)
            {
                ScriptableEnumValueContainer id = idsData[i].Container;
                if (!container.Contains(id))
                {
                    container.Add(id);
                    i++;
                }
                else
                {
                    Debug.LogError($"Duplicate Scriptable({id.name}) found , Deleting Duplicate");
                    idsData.RemoveAt(i);
                }
            }
        }

        private void SearchForDuplicateIds()
        {
            HashSet<string> idv = new HashSet<string>();

            for (int i = 0; i < idsData.Count; i++)
            {
                SystemIdsData systemIdsData = idsData[i];

                if (systemIdsData.Container == null)
                    break;

                List<string> idSources = systemIdsData.Container.Ids;

                for (int j = 0; j < idSources.Count;)
                {
                    string stringIdValue = idSources[j];

                    if (idv.Contains(stringIdValue))
                    {
                        idSources.RemoveAt(j);
                        Debug.LogError($"Duplicate Id entry ({stringIdValue}) found at {systemIdsData.EnumName}, Deleting Duplicate");
                        continue;
                    }

                    if(stringIdValue != string.Empty)
                    idv.Add(stringIdValue);
                    j++;
                }                

            }
        }  
      
    }

    


    [System.Serializable]
    public class SystemIdsData
    {
        [SerializeField] string enumName;
        [SerializeField] ScriptableEnumValueContainer sourceSos;
        
        public string EnumName => enumName;
        public ScriptableEnumValueContainer Container
        {
            get
            {
                return sourceSos;
            }
            set 
            { 
                sourceSos = value;
            }
        }

        public void SetDirty()
        {
            EditorUtility.SetDirty(sourceSos);
        }

    }



    

}