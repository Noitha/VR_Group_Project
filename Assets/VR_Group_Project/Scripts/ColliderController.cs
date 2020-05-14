using System;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class ColliderController : MonoBehaviour
    {
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }
        
        public delegate void OnObjectColliderEnter(GameObject obj);
        public event OnObjectColliderEnter onObjectColliderEnter;
        
        public delegate void OnObjectColliderExit(GameObject obj);
        public event OnObjectColliderExit onObjectColliderExit;

        private void OnCollisionEnter(Collision other)
        {
            onObjectColliderEnter?.Invoke(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            onObjectColliderExit?.Invoke(other.gameObject);
        }
    }
}
