using UnityEngine;
using System.Collections.Generic;
using UnityEditor;




namespace ScriptableEnumSystem
{
    [CreateAssetMenu(fileName = "RuntimeIdContainer", menuName = "Scriptable Objects/ScriptableEnum/RuntimeIdContainer")]
    public class BaseScriptableEnumValueContainer : ScriptableObject
    {
        [SerializeField] protected List<string> _ids;

        public virtual List<string> Ids  {   get { return _ids; }  }


#if UNITY_EDITOR

        public virtual void PreProcessContainer() { }

        private void OnIdsChanged()
        {
            Debug.Log("Call");
            PreProcessContainer();
            ScriptableEnumsContainer.EditorAccessor.ResolvedValue.ValidateContainers();
            MarkDirtyAndSave();
        }
#endif

        public void MarkDirtyAndSave()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}
