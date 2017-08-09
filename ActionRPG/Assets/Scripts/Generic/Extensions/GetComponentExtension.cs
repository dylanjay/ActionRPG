using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class GetComponentExtension
    {
        public static T GetRequiredComponent<T>(this GameObject obj) where T : MonobehaviourExtension
        {
            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError("Expected to find component of type "
                   + typeof(T) + " but found none", obj);
            }

            return component;
        }
    }
}
