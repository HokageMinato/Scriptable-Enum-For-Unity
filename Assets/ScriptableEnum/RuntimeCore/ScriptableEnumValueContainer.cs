using UnityEngine;

using System.Collections.Generic;

namespace ScriptableEnumSystem
{
    [CreateAssetMenu(fileName = nameof(ScriptableEnumValueContainer), menuName = "ScriptableEnum/"+nameof(ScriptableEnumValueContainer))]
    public class ScriptableEnumValueContainer : ScriptableObject
    {
        [SerializeField] private List<string> _ids;

        public List<string> Ids => _ids;

    }
}
