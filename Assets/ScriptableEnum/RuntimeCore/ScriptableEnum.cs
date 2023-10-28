using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableEnumSystem
{
    [System.Serializable]
    public class ScriptableEnum : IEquatable<ScriptableEnum>, IEqualityComparer<ScriptableEnum>
    {

        #region CONSTANTS
        public const string Empty_STRID = "None";
        public const int Empty_INTID = -1;
        public static readonly ScriptableEnum Empty = new ScriptableEnum(Empty_STRID, Empty_INTID);
        #endregion


        #region PRIVATE_VARS
        [SerializeField] private string id;
        [SerializeField] private int fastId = int.MaxValue;

#if UNITY_EDITOR
        [SerializeField] private string menuFilter;
#endif

#if DEBUG_BUILD
        private static bool ShowComparisionLogs = false;
#endif
        #endregion


        #region PUBLIC_PROPERTIES
        public int FastId { get { return fastId; } }
        public string StringId { get { return id; } }


        #endregion

        #region PRIVATE_PROPERTIES
        #endregion

        #region PRIVATE_PROPERTIES

        #endregion

        #region CONSTRUCTOR

        private ScriptableEnum(string id, int unsafeId)
        {
            this.id = id;
            this.fastId = unsafeId;
        }

        public ScriptableEnum(ScriptableEnum source)
        {
            this.id = source.id;
            this.fastId = source.fastId;
        }

        public ScriptableEnum(string id)
        {
            fastId = GetEnumsContainer().GetUnsafeId(id);
            this.id = id;
        }

        public static ScriptableEnum InstantiateRuntimeInstance(string id)
        {
            return new ScriptableEnum(id, GetEnumsContainer().GenerateRuntimeUnsafeIdPair(id));
        }
        #endregion

        #region INTERACE_IMPLEMENTATIONS
        public override string ToString()
        {
            return id;
        }

        public override int GetHashCode()
        {
            return fastId;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ScriptableEnum))
            {

#if DEBUG_BUILD
                throw new Exception($"Trying to check against {obj.GetType()}, Make sure ScriptableEnum derived id is compared and not an object");
#endif

                return false;
            }

            return Equals((ScriptableEnum)obj);
        }

        public bool Equals(ScriptableEnum other)
        {
#if DEBUG_BUILD

            if (ShowComparisionLogs)
                Debug.Log($"Sid{StringId}-Fid{fastId}  ::  Sid{other.StringId}-Fid{other.fastId}");
#endif

            return this == other;
        }

        public bool Equals(ScriptableEnum x, ScriptableEnum y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ScriptableEnum obj)
        {
            return fastId;
        }
        #endregion

        #region OPERATOR_OVERRIDES
        public static bool operator == (ScriptableEnum x, ScriptableEnum y)
        {
            return x.fastId == y.fastId;
        }

        public static bool operator != (ScriptableEnum x, ScriptableEnum y)
        {
            return x.fastId != y.fastId;
        }
        #endregion

        #region PRIVATE_METHODS
        private static ScriptableEnumsContainer GetEnumsContainer()
        { return ScriptableEnumsContainer.EditorOnlyInstance; }
        #endregion

        #if UNITY_EDITOR
        public const string intFieldName = nameof(fastId);
        public const string ValueFieldName = nameof(id);
        public const string menuPathFieldName = nameof(menuFilter);
        #endif

    }
}
