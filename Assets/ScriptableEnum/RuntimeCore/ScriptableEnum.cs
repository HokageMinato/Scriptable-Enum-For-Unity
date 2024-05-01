using ScriptableEnumSystem.CommonDS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableEnumSystem
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class PathFilterAttribute : PropertyAttribute 
    {
        public string PathFilter;

        public PathFilterAttribute(string pathFilter)
        {
            PathFilter = pathFilter;
        }   
    }


    [System.Serializable]
    public class ScriptableEnum : IEquatable<ScriptableEnum>, IEqualityComparer<ScriptableEnum>
    {

        #region CONSTANTS
        public const string Empty_STRID = "None";
        public const int Empty_INTID = -1;
        public static readonly ScriptableEnum Empty = ScriptableEnum.FromStringId(Empty_STRID);
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private string id;
        [SerializeField] private int fastId = int.MaxValue;
        #endregion

        #region PUBLIC_PROPERTIES
        public int FastId { get { return fastId; } }
        public string StringId { get { return id; } }
        #endregion
             

        

        #region CONSTRUCTOR
        public ScriptableEnum(string id)
        {
            this.id = id;
            fastId = GetUnsafeId(id);
        }

        private ScriptableEnum() { }

        public static ScriptableEnum CopyFrom(ScriptableEnum source)
        {
#if DEBUG_BUILD
            CheckForNullSource(source);
#endif

            ScriptableEnum copy = new();
            copy.id = source.id;
            copy.fastId = source.fastId;

            return copy;
        }
       

        public static ScriptableEnum FromStringId(string existingIdString)
        {

#if DEBUG_BUILD
            WarnForNullOrEmpty(existingIdString);
#endif

            if(string.IsNullOrEmpty(existingIdString))
                return Empty;

            ScriptableEnum se = new ScriptableEnum();
            se.fastId = GetUnsafeId(existingIdString);
            se.id = existingIdString;

            return se;

        }

        public static ScriptableEnum Instantiate(string newId) 
        {
            ScriptableEnumsContainer container = null;

#if !UNITY_EDITOR
            container = InstanceTracker<ScriptableEnumsContainer>.Get();
#elif UNITY_EDITOR
            container = ScriptableEnumsContainer.EditorAccessor.ResolvedValue;
#endif
            if (string.IsNullOrEmpty(newId))
                throw new Exception($"{nameof(newId)} is null or empty");

            if (!container.IsIdStringDistinct(newId,out string dupContainerName))
                throw new Exception($"Instantiating duplicate Id \"{newId}\" is not allowed, its present in {dupContainerName}");

            container.GetRuntimeIdContainer().Ids.Add(newId);
            ScriptableEnum newIdS = ScriptableEnum.FromStringId(newId);
            return newIdS;
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
        

        public bool Equals(ScriptableEnum x, ScriptableEnum y)
        {
            return x.Equals(y);
        }

        public bool Equals(ScriptableEnum other)
        {

#if DEBUG_BUILD
            if (ShowComparisionLogs)
                Debug.Log($"Sid{StringId}-Fid{fastId}  ::  Sid{other.StringId}-Fid{other.fastId}");
#endif

            return this == other;
        }


        public int GetHashCode(ScriptableEnum obj)
        {
            return fastId;
        }

        public static int GetUnsafeId(string id)
        {
            if (id == Empty_STRID)
                return Empty_INTID;

           
            return id.GetDeterministicHashCode();
        }


        #endregion

        #region OPERATOR_OVERRIDES
        

        public static bool operator == (ScriptableEnum x, ScriptableEnum y)
        {
            if (x is null && y is null)
                return true;
            
            if (x is null || y is null)
                return false;

            if (x is not ScriptableEnum || y is not ScriptableEnum)
                return false;

            return x.fastId == y.fastId;
        }

        public static bool operator != (ScriptableEnum x, ScriptableEnum y)
        {
            return !(x == y);
        }
        #endregion



#if DEBUG_BUILD
        private static bool ShowComparisionLogs = false;

        private static void CheckForNullSource(ScriptableEnum source)
        {
            if (source == null)
                throw new Exception($"Source Scriptable is null or empty");
        }

        private static void WarnForNullOrEmpty(string existingIdString)
        {
            if (CSExtensions.IsNullOrEmptyOrWhiteSpace(existingIdString))
                Debug.LogError($"{existingIdString} is null or empty");
        }
#endif

#if UNITY_EDITOR

        public string menuFilter;
        public const string intFieldName = nameof(fastId);
        public const string ValueFieldName = nameof(id);
        public const string menuPathFieldName = nameof(menuFilter);

        public void SetMenuFilter(string filter) => menuFilter = filter;
#endif

    }


    [Serializable]
    public enum ScriptableEnumHashStrategy
    {
        STANDARD_DETERMINISTIC = 0,
        ANIMATOR_HASH = 1
    }
}
