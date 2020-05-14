using System;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    [Serializable]
    public struct LevelResetData
    {
        public GameObject levelObject;
        public Vector3 objectPosition;
        public Vector3 objectRotation;
    }
}