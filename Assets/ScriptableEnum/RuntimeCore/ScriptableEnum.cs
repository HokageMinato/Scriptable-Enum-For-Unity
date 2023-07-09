using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableEnumSystem
{
    [System.Serializable]
    public class ScriptableEnum : IEquatable<ScriptableEnum>, IEqualityComparer<ScriptableEnum>
    {
        #region PUBLIC_VARS
        public const int NoneI = 0;
        public const string NoneS = "None";
        public const string SerializedValueName = nameof(value);
        #endregion

        #region PUBLIC_PROPERTIES
        public string Value { get { return value; } }

        public int HashCode
        {
            get
            {
                ValidateHashCode();
                return hashCode;
            }
        }
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private string value;
        private int hashCode = -999;
        #endregion

        #region PUBLIC_METHODS
        public bool Equals(ScriptableEnum other)
        {
            return HashCode == other.HashCode;
        }
        public bool Equals(ScriptableEnum x, ScriptableEnum y)
        {
            return x.Equals(y);
        }
        public int GetHashCode(ScriptableEnum obj)
        {
            return obj.GetHashCode();
        }
        #endregion

        #region PRIVATE_METHODS
        private void ValidateHashCode()
        {
            if (hashCode == -999) 
                hashCode = value.GetHashCode();
        }
        #endregion
    }
}
