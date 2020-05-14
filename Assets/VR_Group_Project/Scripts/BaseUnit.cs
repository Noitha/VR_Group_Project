using System;
using System.Collections.Generic;
using MalbersAnimations;
using UnityEngine;
using UnityEngine.AI;

namespace VR_Group_Project.Scripts
{
    public abstract class BaseUnit : BaseLevelObject
    {
        /// <summary>
        /// NavMeshAgent Component
        /// </summary>
        protected NavMeshAgent NavMeshAgent;
        public UnitUI unitUI;
        protected NavMeshPath NavMeshPath;
        public Camera unitCamera;
        public List<GameObject> runtimeLayerChange = new List<GameObject>();
        #region Nav Mesh Agent

        public bool isControlled;


        /// <summary>
        /// Disable the agent to move.
        /// </summary>
        public void StopAgent()
        {
            NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// Enable the agent to move.
        /// </summary>
        public void StartAgent()
        {
            NavMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Enable the agent component.
        /// </summary>
        public void EnableAgent()
        {
            NavMeshAgent.enabled = true;
        }

        /// <summary>
        /// Disable the agent component.
        /// </summary>
        public void DisableAgent()
        {
            NavMeshAgent.enabled = false;
        }

        #endregion

        public Transform actionDetectionOrigin;
        
        public Vector3 currentCameraOffset;
        
        public Transform cameraContainer;
        public Transform firstPersonCameraOffset;
        public Transform thirdPersonCameraOffset;
        
        public bool inPerspectiveMode;
        
        public MalbersInput malbersInput;

        public bool usedActionButton;
        
        public AnimalType animalType { get; protected set; }
        
        private void Update()
        {
            if (!inPerspectiveMode)
            {
                return;
            }

            cameraContainer.localPosition = currentCameraOffset - unitCamera.transform.localPosition;
            CastOverlapActionDetectionSphere();
        }


        public void UseAction()
        {
            usedActionButton = true;
        }

        private void CastOverlapActionDetectionSphere()
        {
            if (!Physics.Raycast(new Ray(unitCamera.transform.position, unitCamera.transform.forward), out var hit, .2f))
            {
                usedActionButton = false;
                return;
            }

            if (!hit.transform.CompareTag("LevelActionObject"))
            {
                usedActionButton = false;
                return;
            }

            //Display action feedback visual in game.
            
            if (!usedActionButton)
            {
                usedActionButton = false;
                return;
            }

            hit.transform.GetComponent<LevelActionObject>().Action(this);
            usedActionButton = false;
        }
        
        public void ApplyNewPerspectiveView(PerspectiveMode perspectiveMode)
        {
            switch (perspectiveMode)
            {
                case PerspectiveMode.TopView:
                    Player.Instance.playerCamera.enabled = true;
                    unitCamera.enabled = false;
                    inPerspectiveMode = false;
                    break;
                
                case PerspectiveMode.FirstPerson:
                    Player.Instance.playerCamera.enabled = false;
                    unitCamera.enabled = true;
                    currentCameraOffset = firstPersonCameraOffset.localPosition;
                    unitCamera.transform.SetParent(firstPersonCameraOffset);
                    foreach (var excludeRendering in runtimeLayerChange)
                    {
                        excludeRendering.layer = 17;
                    }
                    inPerspectiveMode = true;
                    break;
                
                case PerspectiveMode.ThirdPerson:
                    Player.Instance.playerCamera.enabled = false;
                    unitCamera.enabled = true;
                    currentCameraOffset = thirdPersonCameraOffset.localPosition;
                    unitCamera.transform.SetParent(thirdPersonCameraOffset);
                    foreach (var excludeRendering in runtimeLayerChange)
                    {
                        excludeRendering.layer = 0;
                    }
                    inPerspectiveMode = true;
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(perspectiveMode), perspectiveMode, null);
            }
        }

        public void Move(Vector3 direction)
        {
            NavMeshAgent.Move(direction);
        }

        public void Rotate(float rotate)
        {
            transform.eulerAngles += new Vector3(0, rotate, 0);
        }


        public virtual void TrySetDestination(Vector3 destination)
        {
            NavMeshPath = new NavMeshPath();

            if (NavMeshAgent.CalculatePath(destination, NavMeshPath))
            {
                if (NavMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    NavMeshAgent.Warp(destination);
                }

                //NavMeshAgent.SetPath(NavMeshPath);
            }
        }


        public void EnterControlState()
        {
            enabled = true;
            isControlled = true;
            unitUI.Select();
            Player.Instance.playerCamera.enabled = false;
            Player.Instance.currentControllingUnit = this;
            Player.Instance.isControllingAnUnit = true;
            NavMeshAgent.enabled = true;
        }

        public void ExitControlState()
        {
            isControlled = false;
            unitUI.DeSelect();
            unitCamera.enabled = false;
            Player.Instance.playerCamera.enabled = true;
            Player.Instance.isControllingAnUnit = false;
            malbersInput.ClearInput();
            Player.Instance.currentControllingUnit = null;
            enabled = false;
        }

   
        protected override void GetComponents()
        {
            malbersInput = GetComponent<MalbersInput>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            unitUI = GetComponent<UnitUI>();
        }
        
      
        protected override void InitializeComponents()
        {
            //Set the tag to "Unit".
            tag = "Unit";
            
            //Change the layer to 11 which corresponds to the Unit-Layer.
            gameObject.layer = 11;
            
            //Deselect the UI.
            unitUI.DeSelect();
            
            unitCamera.farClipPlane = 100;
            unitCamera.enabled = false;
        }

        public void ClearDialogue()
        {
            unitUI.dialogueTextBack.text = "";
            unitUI.dialogueTextFront.text = "";
        }

        public void AppendCharacterToDialogue(char character)
        {
            unitUI.dialogueTextBack.text += character;
            unitUI.dialogueTextFront.text += character;
        }
    }
}