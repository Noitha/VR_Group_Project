using System;
using UnityEngine;
using UnityEngine.Events;

namespace VR_Group_Project.Scripts
{
    public class EventController : MonoBehaviour
    {
        public UnityEventVector3 hitPosition;




        public void Initialize()
        {
            if (hitPosition == null)
            {
                hitPosition = new UnityEventVector3();
            }
            
            hitPosition.AddListener(CheckPosition);
        }

        private void CheckPosition(Vector3 hitPos)
        {
            
        }
    }
    
    [Serializable]
    public class UnityEventVector3 : UnityEvent<Vector3> { }
}