using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableEnumSystem.CommonDS
{
    public static class StringExtensions
    {
        public static int GetDeterministicHashCode(this string text)
        {
            unchecked
            {
                int hash = 23;
                ReadOnlySpan<char> textSpan = text.AsSpan();
                for (int i = 0; i < textSpan.Length; i++)
                    hash = hash * 31 + textSpan[i];

                return hash;
            }
        }

        public static string GetStringFromGeneratedDeterministicHashCode(this int generatedHashCode, ScriptableEnumHashStrategy algorithmType = ScriptableEnumHashStrategy.STANDARD_DETERMINISTIC)
        {
#if UNITY_EDITOR
            List<SerializedTuple<string, BaseScriptableEnumValueContainer>> containers = ScriptableEnumsContainer.EditorAccessor.ResolvedValue.ScriptableContainers;
#elif !UNITY_EDITOR
            List<SerializedTuple<string, BaseScriptableEnumValueContainer>> containers = InstanceTracker<ScriptableEnumsContainer>.Get().ScriptableContainers;
#endif

            if (ScriptableEnum.Empty_INTID == generatedHashCode)
                return ScriptableEnum.Empty_STRID;

            string f = string.Empty;
            containers.ForEach(xContainer =>
            {
                xContainer.v2.Ids.ForEach(xId =>
                {
                    if (algorithmType == ScriptableEnumHashStrategy.STANDARD_DETERMINISTIC)
                    {
                        if (xId.GetDeterministicHashCode() == generatedHashCode)
                            f = xId;
                    }

                    if (algorithmType == ScriptableEnumHashStrategy.ANIMATOR_HASH)
                    {
                        if (Animator.StringToHash(xId) == generatedHashCode)
                            f = xId;
                    }
                });
            });

            if (f == string.Empty)
            {
                Debug.LogError($"Unknown inputHash {generatedHashCode}, couldnt find in any container !");
            }

            return f;
        }



        public static bool IsNullOrEmptyOrWhiteSpace(string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        }
    }
}