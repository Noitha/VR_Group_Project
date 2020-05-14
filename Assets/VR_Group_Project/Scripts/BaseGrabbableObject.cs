using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VR_Group_Project.Scripts
{
    public abstract class BaseGrabbableObject : BaseLevelObject
    {
        public AudioClip grabSound;
        public AudioClip collideSound;
        public bool IsEquipped;
        public Controller controller;

        protected Rigidbody objectRigidbody;
        protected AudioSource audioSource;

        private VRTK_InteractableObject _interactableObject;
        protected VRTK_ChildOfControllerGrabAttach _childOfControllerGrabAttach;
        private VRTK_SwapControllerGrabAction _swapControllerGrabAction;
        private VRTK_InteractObjectHighlighter _interactObjectHighlighter;

        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            GetComponents();
            InitializeComponents();
        }

        protected override void GetComponents()
        {
            base.GetComponents();
            
            _childOfControllerGrabAttach = Utility.GetOrAddComponent<VRTK_ChildOfControllerGrabAttach>(gameObject);
            _swapControllerGrabAction = Utility.GetOrAddComponent<VRTK_SwapControllerGrabAction>(gameObject);
            _interactObjectHighlighter = Utility.GetOrAddComponent<VRTK_InteractObjectHighlighter>(gameObject);
            objectRigidbody = Utility.GetOrAddComponent<Rigidbody>(gameObject);
            audioSource = Utility.GetOrAddComponent<AudioSource>(gameObject);
            _interactableObject = Utility.GetOrAddComponent<VRTK_InteractableObject>(gameObject);
        }

        /// <summary>
        /// Initialize the components with correct values.
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            
            //Tag
            tag = "Grabbable";
            
            //Layer
            gameObject.layer = 15;

            //Rigidbody
            objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            objectRigidbody.interpolation = RigidbodyInterpolation.None;
            objectRigidbody.constraints = RigidbodyConstraints.None;
            objectRigidbody.useGravity = true;
            objectRigidbody.isKinematic = false;

            //Interactable object
            _interactableObject.disableWhenIdle = true;
            _interactableObject.allowedNearTouchControllers = VRTK_InteractableObject.AllowedController.Both;
            _interactableObject.allowedTouchControllers = VRTK_InteractableObject.AllowedController.Both;
            _interactableObject.isGrabbable = true;
            _interactableObject.holdButtonToGrab = true;
            _interactableObject.stayGrabbedOnTeleport = true;
            _interactableObject.validDrop = VRTK_InteractableObject.ValidDropTypes.DropAnywhere;
            _interactableObject.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            _interactableObject.allowedGrabControllers = VRTK_InteractableObject.AllowedController.Both;
            _interactableObject.grabAttachMechanicScript = _childOfControllerGrabAttach;
            _interactableObject.secondaryGrabActionScript = _swapControllerGrabAction;
            _interactableObject.isUsable = false;
            _interactableObject.holdButtonToUse = false;
            _interactableObject.useOnlyIfGrabbed = false;
            _interactableObject.pointerActivatesUseAction = false;
            _interactableObject.useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            _interactableObject.allowedUseControllers = VRTK_InteractableObject.AllowedController.Both;
            
            //Child of controller grab attach
            _childOfControllerGrabAttach.precisionGrab = true;

            _interactableObject.enabled = true;
        }

        /// <summary>
        /// A sound to play when colliding
        /// </summary>
        /// <param name="other"></param>
        protected void OnCollisionEnter(Collision other)
        {
            audioSource.PlayOneShot(collideSound);
        }

        public virtual void Grab(Controller c, BaseGrabbableObject baseGrabbableObject)
        {
            IsEquipped = true;
            controller = c;
            //audioSource.PlayOneShot(grabSound);
            
            switch (controller.hand)
            {
                case Hand.Left:
                    controller.player.leftControllerData.hasObjectInHand = true;
                    controller.player.leftControllerData.handObject = baseGrabbableObject;
                    break;
                
                case Hand.Right:
                    controller.player.rightControllerData.hasObjectInHand = true;
                    controller.player.rightControllerData.handObject = baseGrabbableObject;
                    break;
            }
        }

        public virtual void UnGrab()
        {
            IsEquipped = false;
            controller.UnGrab();
            controller = null;
        }

        protected void ForceUnGrab()
        {
            _interactableObject.ForceStopInteracting();
            UnGrab();
        }

        public Level GetLevel()
        {
            return Level;
        }
    }
}