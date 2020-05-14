using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public static class Utility
    {
        public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() == null ? gameObject.AddComponent<T>() : gameObject.GetComponent<T>();
        }
    }
}