using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class NullOrEmptyExtensions
    {
        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null;
        }
        public static bool IsNullOrEmpty(this ICollection obj)
        {
            return obj == null || obj.Count == 0;
        }
        public static bool IsNullOrEmpty<T>(this ICollection<T> obj)
        {
            return obj == null || obj.Count == 0;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AssertNotEmpty<T>(this ICollection<T> array)
        {
            if (array.IsNullOrEmpty())
            {
                Debug.LogErrorFormat("Array found empty or null {0}. Check stack for details." + array.ToString());
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AssertNotNull(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                Debug.LogErrorFormat("Object found null {0}. Check stack for details." + obj.ToString());
            }
        }
    }
}
