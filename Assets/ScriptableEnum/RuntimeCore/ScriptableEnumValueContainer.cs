using UnityEngine;
using System.Collections.Generic;

namespace ScriptableEnumSystem
{
    [CreateAssetMenu(fileName = "RuntimeIdContainer", menuName = "Scriptable Objects/ScriptableEnum/RuntimeIdContainer")]
    public class BaseScriptableEnumValueContainer : ScriptableObject
    {
        [SerializeField] protected List<string> _ids;

        public virtual List<string> Ids  {   get { return _ids; }  }

    }
}
