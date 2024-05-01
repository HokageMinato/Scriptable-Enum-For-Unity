using UnityEngine;
using System.Collections.Generic;

using System;
using UnityEditor;
using System.Linq;
using ScriptableEnumSystem.CommonDS;
using StringExtensions = ScriptableEnumSystem.CommonDS.StringExtensions;

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
        [SerializedTupleLabels("Menu Path", "Scriptable Enum Asset")]
        [SerializeField] private List<SerializedTuple<string, BaseScriptableEnumValueContainer>> scriptableContainersData;
        #endregion


        #region PUBLIC_PROPERTIES
        public List<SerializedTuple<string, BaseScriptableEnumValueContainer>> ScriptableContainers { get { return scriptableContainersData; } }

        public bool IsDirty { get; set; }
        #endregion


        #region PUBLIC_METHODS
        public bool IsIdStringDistinct(string strId,out string containerName)
        {
            containerName = "";

            for (int i = 0; i < scriptableContainersData.Count; i++)
            {

                BaseScriptableEnumValueContainer container = scriptableContainersData[i].v2;
                containerName = container.name;
                List<string> containers = container.Ids;
                for (int j = 0; j < containers.Count; j++)
                {
                    if (containers[j] == strId)
                        return false;
                }
            }

            return true;

        }

        public BaseScriptableEnumValueContainer GetRuntimeIdContainer()
        {
            SerializedTuple<string, BaseScriptableEnumValueContainer> runtimeContainerPair = scriptableContainersData[scriptableContainersData.Count - 1];

            if (!runtimeContainerPair.v1.Contains(RUNTIME_CONTAINER_MENU_PATH))
                throw new Exception($"Runtime Not Assigned, Make sure a container is attached with path {RUNTIME_CONTAINER_NAME} and its name set to {RUNTIME_CONTAINER_NAME}");

            return runtimeContainerPair.v2;


        }

        public void ResetRuntimeValues()
        {
            GetRuntimeIdContainer().Ids.Clear();
        }
        #endregion


#if UNITY_EDITOR

        public static SingleSearchField<ScriptableEnumsContainer> EditorAccessor = new(() =>
        {
            return AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>("Assets/Settings/ScriptableEnum/_ScriptableEnumsContainer.asset");

        }, () => { return Application.isPlaying; });

        public int DrawerVersion = 0;

        public bool IsContainerStateValid()
        {

            BaseScriptableEnumValueContainer firstContainer = scriptableContainersData[0].v2;

            if (firstContainer.name != DEFAULT_CONTAINER_NAME)
            {
                Debug.LogError($"Make sure first container is {DEFAULT_CONTAINER_NAME}, Also do not rename it incase done so");
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
            

            BaseScriptableEnumValueContainer lastContainer = scriptableContainersData[scriptableContainersData.Count - 1].v2;
            if (lastContainer.name != RUNTIME_CONTAINER_NAME)
            {
                Debug.LogError($"Make sure the last container is '{RUNTIME_CONTAINER_NAME}', since \"{lastContainer.name}\" doesnt match \"{RUNTIME_CONTAINER_NAME}\"");
                return false;
            }


            return true;
        }


        [ContextMenu(nameof(ValidateContainers))]
        public void ValidateContainers()
        {
            if (IsContainerStateValid())
            {
                SearchForDuplicateScriptables();
                SearchForDuplicateIds();
                BaseScriptableEnumValueContainer test = GetRuntimeIdContainer();
            }
        }

        [ContextMenu(nameof(ForcePreProcessContainers))]
        public void ForcePreProcessContainers() 
        {
            foreach (var item in scriptableContainersData)
                item.v2.PreProcessContainer();

            DrawerVersion++;
        }

        private void SearchForDuplicateScriptables()
        {

            HashSet<BaseScriptableEnumValueContainer> distinctContainersHashSet = new HashSet<BaseScriptableEnumValueContainer>();
            for (int i = 0; i < scriptableContainersData.Count; i++)
            {
                BaseScriptableEnumValueContainer scriptableContainer = scriptableContainersData[i].v2;

                if (!distinctContainersHashSet.Contains(scriptableContainer))
                    distinctContainersHashSet.Add(scriptableContainer);
                else
                    Debug.LogError($"Duplicate Scriptable({scriptableContainer.name}) found , Deleting Duplicate");
            }
        }

        private void SearchForDuplicateIds()
        {
            Dictionary<string, string> idMappedToContainerDict = new Dictionary<string, string>();

            for (int i = 0; i < scriptableContainersData.Count; i++)
            {
                SerializedTuple<string, BaseScriptableEnumValueContainer> scriptableContainerPair = scriptableContainersData[i];

                if (scriptableContainerPair.v2 == null)
                    throw new Exception($"Null entry for path \" {scriptableContainerPair.v1}\" ");


                List<string> idValues = scriptableContainerPair.v2.Ids;
                BaseScriptableEnumValueContainer scriptableContainer = scriptableContainerPair.v2;

                CheckForEmptyEntries(idValues, scriptableContainer);
                CheckForDuplicateIds(idMappedToContainerDict, scriptableContainerPair, idValues, scriptableContainer);
                CheckForHashCollision(idMappedToContainerDict.Values.ToList());

            }
        }
        private static void CheckForEmptyEntries(List<string> idValues, BaseScriptableEnumValueContainer scriptableContainer)
        {
            if (idValues.FirstOrDefault(xId => StringExtensions.IsNullOrEmptyOrWhiteSpace(xId)) != null)
                Debug.LogError($"Empty Id Found in container \"{scriptableContainer.name}\"");
        }

        private static void CheckForDuplicateIds(Dictionary<string, string> idMappedToContainerDict, SerializedTuple<string, BaseScriptableEnumValueContainer> scriptableContainerPair, List<string> idValues, BaseScriptableEnumValueContainer scriptableContainer)
        {
            idValues.ForEach(stringIdValue =>
            {
                if (!idMappedToContainerDict.ContainsKey(stringIdValue))
                    idMappedToContainerDict.Add(stringIdValue, scriptableContainer.name);
                else
                {
                    if (scriptableContainer.name != RUNTIME_CONTAINER_NAME)
                    {
                        Debug.LogError($"Duplicate Id entry ({stringIdValue}) found at {scriptableContainerPair.v1} which collides with \"{idMappedToContainerDict[stringIdValue]}\" container's value");
                        return;
                    }
                }

            });
        }

        private static void CheckForHashCollision(List<string> idMappedToContainerDict)
        {
            Dictionary<int, string> seenHashes = new Dictionary<int, string>();

            foreach (var idValue in idMappedToContainerDict)
            {
                int hashCode = idValue.GetDeterministicHashCode();

                if (seenHashes.TryGetValue(hashCode, out string existingValue))
                {
                    Debug.LogError($"Hash collision detected between values '{existingValue}' and '{idValue}' with hash code {hashCode}");
                }
                else
                {
                    seenHashes.Add(hashCode, idValue);
                }
            }
        }

#endif

    }




}
