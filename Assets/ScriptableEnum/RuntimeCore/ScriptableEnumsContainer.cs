using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace ScriptableEnumSystem
{

    /// <summary>
    ///
    /// ASSOCIATED RISKS:
    /// 1) Since the unsafe ID is based upon an incrementing integer
    ///    Any change such as deletion of an instance or rearraning the list
    ///    would cause them to be non-deteministic.
    ///    This means that the binding will only be preserved
    ///    at application-lifecycle level.
    ///    
    /// 2) Creating ScriptableEnum from string.
    ///    At Many places it will be required to have ScriptableEnum instance generated 
    ///    from strings or create copy of SEnums from other SEnums,
    ///    Since container will be present at runtime it will be easy 
    ///    
    /// 3) Instantiating instance at runtime.
    ///                 
    /// 
    /// </summary>


    public class ScriptableEnumsContainer : ScriptableObject
    {

        public const string DEFAULT_CONTAINER_MENU_PATH = "Default";
        public const string RUNTIME_CONTAINER_MENU_PATH = "RuntimeGenerated";
        public const string DEFAULT_CONTAINER_NAME = "_ScriptableId_Default";
        public const string RUNTIME_CONTAINER_NAME = "_ScriptableId_Runtime";


        #region PRIVATE_SERIALIZED_VARS
        
        [SerializeField] private List<PathAssetPair> scriptableContainersData;
        #endregion


        #region PRIVATE_VARS
        #endregion

        #region PUBLIC_VARS
        [SerializeField] public List<StringIntPair> unsafeIdLkp;
        #endregion


        #region PUBLIC_PROPERTIES
        public List<PathAssetPair> ScriptableContainers
        {
            get { return scriptableContainersData; }
        }
        #endregion


        #region PUBLIC_METHODS
        [ContextMenu("Refresh")]
        public void RegenerateIdCache()
        {
            CheckValidity();
            SearchForDuplicateScriptables();
            SearchForDuplicateIds();
            SetUnsafeIds();
            var test = GetRuntimeIdTuple();
        }


        public int GenerateRuntimeUnsafeIdPair(string strId)
        {
            Debug.Log("invoked");
            GetRuntimeIdTuple().v2.Ids.Add(strId);
            RegenerateIdCache();
            return GetUnsafeId(strId);
        }

        public int GetUnsafeId(string id)
        {
            foreach (var item in unsafeIdLkp)
            {
                if (item.v1 == id)
                    return item.v2;
            }

            throw new System.Exception($"Make sure requested id {id} is present in value container," +
                                       $" if this occured at runtime REVERT the build and upload container with all the IDs");
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetUnsafeIds()
        {
            int c = ScriptableEnum.Empty_INTID + 1;
            unsafeIdLkp.Clear();
            foreach (var scriptableContainerPairs in scriptableContainersData)
            {
                var valueContainer = scriptableContainerPairs.v2;

                foreach (var id in valueContainer.Ids)
                {
                    if (HandledAsDefault(id))
                        continue;

                    unsafeIdLkp.Add(new(id, c));
                    c++;
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private bool HandledAsDefault(string id)
        {
            if (id == ScriptableEnum.Empty_STRID)
            {
                unsafeIdLkp.Add(new(id, ScriptableEnum.Empty_INTID));
                return true;
            }

            return false;
        }

        private PathAssetPair GetRuntimeIdTuple()
        {
            foreach (var scriptableContainerPair in scriptableContainersData)
            {
                if (scriptableContainerPair.v1.Contains(RUNTIME_CONTAINER_MENU_PATH))
                    return scriptableContainerPair;
            }

            throw new Exception($"Runtime Not Assigned, Make sure a container is attached with path \"{RUNTIME_CONTAINER_MENU_PATH}\" and its name set to \"{RUNTIME_CONTAINER_NAME}\"");

        }


        private void SearchForDuplicateScriptables()
        {

            HashSet<BaseScriptableEnumValueContainer> distinctContainersHashSet = new HashSet<BaseScriptableEnumValueContainer>();
            for (int i = 0; i < scriptableContainersData.Count;)
            {
                BaseScriptableEnumValueContainer scriptableContainer = scriptableContainersData[i].v2;

                if (!distinctContainersHashSet.Contains(scriptableContainer))
                {
                    distinctContainersHashSet.Add(scriptableContainer);
                    i++;
                }
                else
                {
                    Debug.LogError($"Duplicate Scriptable({scriptableContainer.name}) found , Deleting Duplicate");
                    scriptableContainersData.RemoveAt(i);
                }
            }
        }

        private void SearchForDuplicateIds()
        {
            Dictionary<string, string> idMappedToContainerDict = new Dictionary<string, string>();

            for (int i = 0; i < scriptableContainersData.Count; i++)
            {
                var scriptableContainerPair = scriptableContainersData[i];

                if (scriptableContainerPair.v2 == null)
                    throw new Exception($"Null entry for path \" {scriptableContainerPair.v1}\" ");


                List<string> idValue = scriptableContainerPair.v2.Ids;
                var scriptableContainer = scriptableContainerPair.v2;
                int INFILOOP_CTR = 0;

                for (int j = 0; j < idValue.Count;)
                {

#if DEBUG_BUILD
                    if (idValue[j] == string.Empty)
                    {
                        throw new Exception($"Empty Id Found at Index \"{j}\" in container \"{scriptableContainer.name}\"");
                    }
#endif

                    string stringIdValue = idValue[j];

                    if (idMappedToContainerDict.ContainsKey(stringIdValue))
                    {

                        #region DEBUG_CODE
#if DEBUG_BUILD
                        if (scriptableContainer.name != RUNTIME_CONTAINER_NAME)
                            throw new Exception($"Duplicate Id entry ({stringIdValue}) found at {scriptableContainerPair.v1} which collides with \"{idMappedToContainerDict[stringIdValue]}\" container's value");

                        Debug.Log($"Removing {stringIdValue} Duplicate from {RUNTIME_CONTAINER_NAME}");

                        INFILOOP_CTR++;
                        if (INFILOOP_CTR > 900)
                        {
                            throw new Exception($"INFI LOOK ERROR AT {nameof(SearchForDuplicateIds)}");
                        }

#endif
                        #endregion

                        idValue.RemoveAt(j);
                        continue;
                    }



                    idMappedToContainerDict.Add(stringIdValue, scriptableContainer.name);

                    j++;


                }

            }
        }

#if UNITY_EDITOR

        private static ScriptableEnumsContainer instance_editorOnly;
        
        public static ScriptableEnumsContainer EditorOnlyInstance
        {
            get 
            {
                if (instance_editorOnly == null) 
                {
                   instance_editorOnly =  AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:ScriptableEnumsContainer")[0]));
                }

                return instance_editorOnly;
            }
        }

        

        public bool CheckValidity()
        {
            var firstContainer = scriptableContainersData[0].v2;

            if (firstContainer.name != DEFAULT_CONTAINER_NAME)
            {
                Debug.LogError($"Make sure first container is \"{DEFAULT_CONTAINER_NAME}\", and path \"{DEFAULT_CONTAINER_MENU_PATH}\" Also do not rename it incase done so");
                return false;
            }

            if (firstContainer.Ids == null || firstContainer.Ids.Count == 0)
            {
                Debug.LogError($"Make sure the container '{DEFAULT_CONTAINER_NAME}' is initialized with default Id");
                return false;
            }

            if (firstContainer.Ids[0] != ScriptableEnum.Empty_STRID)
            {
                Debug.LogError($"Make sure the container '{DEFAULT_CONTAINER_NAME}''s  first Id is value {ScriptableEnum.Empty_STRID}");
                return false;
            }


            var lastContainer = scriptableContainersData[scriptableContainersData.Count - 1].v2;
            if (lastContainer.name != RUNTIME_CONTAINER_NAME)
            {
                Debug.LogError($"Make sure the 'LAST' container is '{RUNTIME_CONTAINER_NAME}', since \"{lastContainer.name}\" doesnt match \"{RUNTIME_CONTAINER_NAME}\"");
                return false;
            }


            return true;
        }
#endif

        #endregion

        #region INNER_CLASS_DEFNINITIONS
        [Serializable]
        public class StringIntPair 
        {
            public string v1;
            public int v2;

            public StringIntPair(string id, int c)
            {
                v1 = id;
                v2 = c;
            }
        }
        #endregion
    }

    [Serializable]
    public class PathAssetPair
    {
        public string v1;
        public BaseScriptableEnumValueContainer v2;
    }

}