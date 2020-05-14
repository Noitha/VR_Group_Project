using System.Collections.Generic;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class PressurePlate : MonoBehaviour
    {
        [Header("The Trigger GameObject.")]
        public Transform triggerTransform;
        
        [Header("Trigger Controller.")]
        public TriggerController triggerController;

        [Header("Audio")] 
        private AudioSource _audioSource;
        public AudioClip activateSound;
        public AudioClip deactivateSound;

        public MeshRenderer stateVisual;
        
        public bool IsActive { get; private set; }
        public List<BaseUnit> unitsOnPlatform;

        public BoxCollider boxCollider;
        
        public delegate void Notify();
        public event Notify onPlatformActivate;
        public event Notify onPlatformDesActivate;

        private void Start()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
            stateVisual.gameObject.layer = 2;
            IsActive = false;
            unitsOnPlatform = new List<BaseUnit>();

            triggerController.onObjectTriggerEnter += delegate(GameObject obj)
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }
                
                unitsOnPlatform.Add(unit);
                Check();
            };
            
            triggerController.onObjectTriggerExit += delegate(GameObject obj)
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }

                unitsOnPlatform.Remove(unit);
                Check();
            };
            
            Check();
        }

        private void Check()
        {
            if (unitsOnPlatform.Count > 0 && !IsActive)
            {
                IsActive = true;
                onPlatformActivate?.Invoke();
                _audioSource.PlayOneShot(activateSound);
            }
            else
            {
                IsActive = false;
                onPlatformDesActivate?.Invoke();
                _audioSource.PlayOneShot(deactivateSound);
            }
        }
        
        public void Lock()
        {
            stateVisual.enabled = true;
            boxCollider.isTrigger = false;
        }
        
        public void UnLock()
        {
            stateVisual.enabled = false;
            boxCollider.isTrigger = true;
        }
    }
}