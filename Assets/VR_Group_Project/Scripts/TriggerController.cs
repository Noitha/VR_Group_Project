using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class TriggerController : MonoBehaviour
    {
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }
        
        public delegate void OnObjectTriggerEnter(GameObject obj);
        public event OnObjectTriggerEnter onObjectTriggerEnter;
        
        public delegate void OnObjectTriggerExit(GameObject obj);
        public event OnObjectTriggerExit onObjectTriggerExit;
        
        private void OnTriggerEnter(Collider other)
        {
            onObjectTriggerEnter?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            onObjectTriggerExit?.Invoke(other.gameObject);
        }
    }
}