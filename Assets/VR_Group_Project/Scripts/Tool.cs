using System;
using UnityEngine;
using VRTK;

namespace VR_Group_Project.Scripts
{
    public class Tool : BaseGrabbableObject
    {
        public ToolType toolType;
        private TriggerController _triggerController;

        public AudioClip toolSound;

        protected override void GetComponents()
        {
            base.GetComponents();
            _triggerController = GetComponentInChildren<TriggerController>();
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            _childOfControllerGrabAttach.precisionGrab = false;
            
            _triggerController.onObjectTriggerEnter += delegate(GameObject obj) 
            { 
                audioSource.PlayOneShot(toolSound);
                
                if (!IsEquipped)
                {
                    return;
                }
                
                switch (controller.hand)
                {
                    case Hand.Left:
                        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerReferenceLeftHand(), 1, .2f, .2f);
                        break;
                    
                    case Hand.Right:
                        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerReferenceRightHand(), 1, .2f, .2f);
                        break;
                    
                    default: throw new ArgumentOutOfRangeException();
                }

                var transformableComponent = obj.GetComponent<TransformableObject>();

                if (transformableComponent == null)
                {
                    return;
                }
            
                transformableComponent.Hit(this);
            };
        }
    }
}