using System;
using UnityEngine;
using VRTK;

namespace VR_Group_Project.Scripts
{
    public class Player : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static Player Instance;
         
        [Serializable]
        public struct ControllerData
        {
            //The controller reference.
            public Controller controller;
            
            //The line renderer component.
            public LineRenderer lineRenderer;
            
            //Is the current raycast navigable.
            public bool currentRaycastHitIsNavigable;
            //The raycast hit data.
            public RaycastHit raycastHit;
            
            //Is the current raycast interactable
            public bool currentRaycastHitIsInteractable;
            //The actual interactable object being raycast. 
            public LevelActionObject currentRayCastedLevelActionObject;
            
            //Is the current raycast hit a unit.
            public bool currentRaycastHitIsUnit;
            //What unit is being raycast.
            public BaseUnit currentRayCastedUnit;
            
            //Has the controller an object in the hand.
            public bool hasObjectInHand;
            //What object does the player hold.
            public BaseGrabbableObject handObject;
            
            public void ResetRaycast()
            {
                lineRenderer.enabled = false;
                currentRaycastHitIsInteractable = false;
                currentRaycastHitIsUnit = false;
                currentRaycastHitIsNavigable = false;
            }
        }
        
        public ControllerData leftControllerData;
        public ControllerData rightControllerData;
        
        //The transform of the camera.
        public Transform cameraTransform;

        /// <summary>
        ///Is the player inside the level boundaries.
        /// </summary>
        public bool isInsideLevel;

        public LayerMask ignoreRaycastLevelDetermination;
        public LayerMask ignoreRaycastLevelObjects;
        
        private Rigidbody playerRigidbody;
        
        public Camera playerCamera;

        public BaseUnit currentControllingUnit;
        public bool isControllingAnUnit;
        
        [Header("Movement Settings")]
        public MovementSettings insideLevel;
        public MovementSettings outsideLevel;

        public float unitMoveSpeed;
        public int unitRotationSpeed;

        //The current inputs at runtime.
        public Vector2 moveInput;
        private float yRotationInput;
        private float liftInput;

        public Level currentLevel;
        private bool _anyControllerInLevel;
        
        public AudioSource audioSource;
        


        public Material rayCastingNavMeshMaterial;
        public Material rayCastingUnitMaterial;
        public Material rayCastingGrabbableObjectMaterial;
        public Material rayCastingLevelActionObjectMaterial;

        public PerspectiveMode perspectiveMode;

        #endregion

        private void FixedUpdate()
        {
            GetInput();
            ApplyControls();
            GetRaycast();
        }
        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            GetComponents();
            InitializeComponents();
        }
        private void GetComponents()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }
        private void InitializeComponents()
        {
            perspectiveMode = PerspectiveMode.TopView;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            playerRigidbody.useGravity = false;
            playerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            SetupControls();
        }
        private void SetupControls()
        {
            leftControllerData.controller.Initialize(this, Hand.Left);
            rightControllerData.controller.Initialize(this, Hand.Right);

            //Left controller - grab
            leftControllerData.controller.GetInteractGrab().ControllerGrabInteractableObject += delegate(object sender, ObjectInteractEventArgs objectInteractEventArgs)
            {
                var grabbableObject = objectInteractEventArgs.target.GetComponent<BaseGrabbableObject>();

                if (grabbableObject == null)
                {
                    return;
                }

                grabbableObject.Grab(leftControllerData.controller, grabbableObject);

                if (grabbableObject is Tool)
                {
                    return;
                }
                    
                foreach (var unit in grabbableObject.GetLevel().UnitsInLevel)
                {
                    unit.EnableAgent();
                }
            };

            //Left controller - un grab
            leftControllerData.controller.GetInteractGrab().ControllerUngrabInteractableObject += delegate(object sender, ObjectInteractEventArgs objectInteractEventArgs)
            {
                var grabbableObject = objectInteractEventArgs.target.GetComponent<BaseGrabbableObject>();

                if (grabbableObject == null)
                {
                    return;
                }

                grabbableObject.UnGrab();

                if (grabbableObject is PlaceableNavigation)
                {
                    StartCoroutine(grabbableObject.GetLevel().RebuildNavMeshAfterDelay());
                }
            };

            //Right controller - grab
            rightControllerData.controller.GetInteractGrab().ControllerGrabInteractableObject +=
                delegate(object sender, ObjectInteractEventArgs objectInteractEventArgs)
                {
                    var grabbableObject = objectInteractEventArgs.target.GetComponent<BaseGrabbableObject>();

                    if (grabbableObject == null)
                    {
                        return;
                    }

                    grabbableObject.Grab(rightControllerData.controller, grabbableObject);

                    if (grabbableObject is Tool)
                    {
                        return;
                    }
                    
                    foreach (var unit in grabbableObject.GetLevel().UnitsInLevel)
                    {
                        unit.DisableAgent();
                    }
                };

            //Right controller - un grab
            rightControllerData.controller.GetInteractGrab().ControllerUngrabInteractableObject += delegate(object sender, ObjectInteractEventArgs objectInteractEventArgs)
            {
                var grabbableObject = objectInteractEventArgs.target.GetComponent<BaseGrabbableObject>();

                if (grabbableObject == null)
                {
                    return;
                }

                grabbableObject.UnGrab();

                if (grabbableObject is PlaceableNavigation)
                {
                    StartCoroutine(grabbableObject.GetLevel().RebuildNavMeshAfterDelay());
                }
            };
            
            //Left controller - interact
            leftControllerData.controller.GetControllerEvents().TriggerPressed += LeftInteract;
            
            //Right controller - interact
            rightControllerData.controller.GetControllerEvents().TriggerPressed += RightInteract;

            //Right controller - unit selection
            rightControllerData.controller.GetControllerEvents().ButtonOnePressed += delegate
            {
                if (!_anyControllerInLevel && currentLevel == null)
                {
                    return;
                }

                if (!rightControllerData.currentRaycastHitIsUnit)
                {
                    return;
                }

                var unit = rightControllerData.currentRayCastedUnit;
            
                if (isControllingAnUnit)
                {
                    currentControllingUnit.ExitControlState();
                }
                
                unit.EnterControlState();
                unit.ApplyNewPerspectiveView(perspectiveMode);
            };

            //Right controller - unit deselection
            rightControllerData.controller.GetControllerEvents().ButtonTwoPressed += delegate
            {
                if (!isControllingAnUnit)
                {
                    return;
                }
                
                currentControllingUnit.ExitControlState();
            };
            
            //Left controller - change perspective
            leftControllerData.controller.GetControllerEvents().ButtonTwoPressed += ChangePerspectiveMode;
        }
        
        private void ChangePerspectiveMode(object sender, ControllerInteractionEventArgs args)
        {
            if (!isControllingAnUnit)
            {
                return;
            }
            
            if ((int) perspectiveMode == Enum.GetValues(typeof(PerspectiveMode)).Length - 1)
            {
                perspectiveMode = 0;
            }
            else
            {
                perspectiveMode++;
            }
            
            currentControllingUnit.ApplyNewPerspectiveView(perspectiveMode);
        }

        /// <summary>
        /// Return true if holding any object.
        /// </summary>
        /// <returns></returns>
        public bool HasObjectInHand()
        {
            return leftControllerData.hasObjectInHand || rightControllerData.hasObjectInHand;
        }

        private void LeftInteract(object sender, ControllerInteractionEventArgs e)
        {
            
        }
        
        private void RightInteract(object sender, ControllerInteractionEventArgs e)
        {
            if (!isControllingAnUnit)
            {
                return;
            }

            currentControllingUnit.UseAction();
        }

        /// <summary>
        /// Get the inputs from the controllers.
        /// </summary>
        private void GetInput()
        {
            moveInput = leftControllerData.controller.GetControllerEvents().GetTouchpadAxis();
            yRotationInput = rightControllerData.controller.GetControllerEvents().GetTouchpadAxis().x;
            liftInput = rightControllerData.controller.GetControllerEvents().GetTouchpadAxis().y;
        }

        /// <summary>
        /// Apply the gathered inputs.
        /// </summary>
        private void ApplyControls()
        {
            if (isControllingAnUnit)
            {
                playerRigidbody.velocity = Vector3.zero;
                currentControllingUnit.Rotate(yRotationInput * unitRotationSpeed * Time.deltaTime);
                currentControllingUnit.malbersInput.SetInput();
                return;
            }
            
            var playerDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
            playerRigidbody.velocity = Vector3.ProjectOnPlane(playerDirection * (isInsideLevel ? insideLevel.moveSpeed : outsideLevel.moveSpeed), Vector3.up);

            if (Mathf.Abs(liftInput) > .5f)
            {
                playerRigidbody.velocity += new Vector3(0, liftInput * (isInsideLevel ? insideLevel.liftSpeed : outsideLevel.liftSpeed), 0);
            }

            if (Mathf.Abs(yRotationInput) > .5f)
            {
                transform.eulerAngles += new Vector3(0, yRotationInput * Time.deltaTime * (isInsideLevel ? insideLevel.rotationSpeed : outsideLevel.rotationSpeed), 0);
            }
        }

        /// <summary>
        /// Ignore when holding an object.
        /// Show or hide the line renderer at the hit position when pointing at a grabbable object or a navigable path.
        /// Set the raycastHit, the validRaycast boolean and validNavigable boolean accordingly.
        /// </summary>
        private void GetRaycast()
        {
            leftControllerData.ResetRaycast();
            rightControllerData.ResetRaycast();

            if (HasObjectInHand())
            {
                return;
            }

            var leftRay = new Ray(leftControllerData.controller.GetTransform().position, leftControllerData.controller.GetTransform().forward);
            var rightRay = new Ray(rightControllerData.controller.GetTransform().position, rightControllerData.controller.GetTransform().forward);

            //Only raycast if no controllers are in the level. In that case raycast onto the glass.
            if (!_anyControllerInLevel)
            {
                currentLevel = Physics.Raycast(leftRay, out var leftHit, 2f, ignoreRaycastLevelDetermination)
                    ? leftHit.transform.GetComponent<Level>()
                    : null;
                
                currentLevel = Physics.Raycast(rightRay, out var rightHit, 2f, ignoreRaycastLevelDetermination)
                    ? rightHit.transform.GetComponent<Level>()
                    : null;
            }

            if (Physics.Raycast(leftRay, out var otherLeftHit, 2f, ignoreRaycastLevelObjects))
            {
                leftControllerData.lineRenderer.SetPosition(0, leftControllerData.controller.GetTransform().position);
                leftControllerData.lineRenderer.SetPosition(1, otherLeftHit.point);
                
                switch (otherLeftHit.transform.gameObject.layer)
                {
                    //Placeable Navigation
                    case 9:
                        leftControllerData.lineRenderer.material = rayCastingGrabbableObjectMaterial;
                        leftControllerData.currentRaycastHitIsNavigable = true;
                        leftControllerData.raycastHit = otherLeftHit;
                        break;

                    //Nav Mesh
                    case 10:
                        leftControllerData.lineRenderer.material = rayCastingNavMeshMaterial;
                        leftControllerData.currentRaycastHitIsNavigable = true;
                        leftControllerData.raycastHit = otherLeftHit;
                        break;

                    //Unit
                    case 11:
                        leftControllerData.lineRenderer.material = rayCastingUnitMaterial;
                        leftControllerData.currentRaycastHitIsUnit = true;
                        leftControllerData.currentRayCastedUnit = otherLeftHit.transform.GetComponent<BaseUnit>();
                        break;

                    //Level Action Object 
                    case 14:
                        leftControllerData.lineRenderer.material = rayCastingLevelActionObjectMaterial;
                        leftControllerData.currentRaycastHitIsInteractable = true;
                        leftControllerData.currentRayCastedLevelActionObject = otherLeftHit.transform.GetComponent<LevelActionObject>();
                        break;
                }
                
                leftControllerData.lineRenderer.enabled = true;
            }
            
            if (Physics.Raycast(rightRay, out var otherRightHit, 2f, ignoreRaycastLevelObjects))
            {
                rightControllerData.lineRenderer.SetPosition(0, rightControllerData.controller.GetTransform().position);
                rightControllerData.lineRenderer.SetPosition(1, otherRightHit.point);
                
                switch (otherRightHit.transform.gameObject.layer)
                {
                    //Placeable Navigation
                    case 9:
                        rightControllerData.lineRenderer.material = rayCastingGrabbableObjectMaterial;
                        rightControllerData.currentRaycastHitIsNavigable = true;
                        rightControllerData.raycastHit = otherRightHit;
                        break;

                    //Nav Mesh
                    case 10:
                        rightControllerData.lineRenderer.material = rayCastingNavMeshMaterial;
                        rightControllerData.currentRaycastHitIsNavigable = true;
                        rightControllerData.raycastHit = otherRightHit;
                        break;

                    //Unit
                    case 11:
                        rightControllerData.lineRenderer.material = rayCastingUnitMaterial;
                        rightControllerData.currentRaycastHitIsUnit = true;
                        rightControllerData.currentRayCastedUnit = otherRightHit.transform.GetComponent<BaseUnit>();
                        break;

                    //Level Action Object 
                    case 14:
                        rightControllerData.lineRenderer.material = rayCastingLevelActionObjectMaterial;
                        rightControllerData.currentRaycastHitIsInteractable = true;
                        rightControllerData.currentRayCastedLevelActionObject = otherRightHit.transform.GetComponent<LevelActionObject>();
                        break;
                }
                
                rightControllerData.lineRenderer.enabled = true;
            }
        }
        
        /// <summary>
        /// Determine the level by checking if a controller is inside the level boundary.
        /// </summary>
        public void DetermineLevel()
        {
            //Both controllers are in the same level.
            if (leftControllerData.controller.level != null && rightControllerData.controller.level != null &&
                leftControllerData.controller.level == rightControllerData.controller.level)
            {
                _anyControllerInLevel = true;
                currentLevel = leftControllerData.controller.level;
                return;
            }

            //Controllers are in two different levels.
            if (leftControllerData.controller.level != null && rightControllerData.controller.level != null &&
                leftControllerData.controller.level != rightControllerData.controller.level)
            {
                _anyControllerInLevel = false;
                currentLevel = null;
                return;
            }

            //Only the left controller is in the level.
            if (leftControllerData.controller.level != null && rightControllerData.controller.level == null)
            {
                _anyControllerInLevel = true;
                currentLevel = leftControllerData.controller.level;
                return;
            }

            //Only the right controller is in the level. 
            if (leftControllerData.controller.level == null && rightControllerData.controller.level != null)
            {
                _anyControllerInLevel = true;
                currentLevel = rightControllerData.controller.level;
            }
        }

        public void UnGrab(Hand hand)
        {
            switch (hand)
            {
                case Hand.Left:
                    leftControllerData.hasObjectInHand = false;
                    leftControllerData.handObject = null;
                    break;
                
                case Hand.Right:
                    rightControllerData.hasObjectInHand = false;
                    rightControllerData.handObject = null;
                    break;
            }
        }
    }
}