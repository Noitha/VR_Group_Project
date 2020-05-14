using TMPro;
using UnityEngine;
using VRTK;
using VRTK.Controllables.PhysicsBased;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VR_Group_Project.Scripts
{
    public class LevelStartReset : BaseLevelObject
    {
        private VRTK_PhysicsRotator _physicsRotator;
        private VRTK_InteractableObject _interactableObject;
        private VRTK_RotatorTrackGrabAttach _rotatorTrackGrabAttach;
        private VRTK_SwapControllerGrabAction _swapControllerGrabAction;


        public TextMeshProUGUI resetText;
        public TextMeshProUGUI startText;

        private bool _resetIsSelected;
        private bool _startIsSelected;
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            _physicsRotator = GetComponentInChildren<VRTK_PhysicsRotator>();
            _interactableObject = GetComponentInChildren<VRTK_InteractableObject>();
            _rotatorTrackGrabAttach = GetComponentInChildren<VRTK_RotatorTrackGrabAttach>();
            _swapControllerGrabAction = GetComponentInChildren<VRTK_SwapControllerGrabAction>();
                
            _interactableObject.grabAttachMechanicScript = _rotatorTrackGrabAttach;
            _interactableObject.secondaryGrabActionScript = _swapControllerGrabAction;

            resetText.enabled = false;
            startText.enabled = false;
            
            _physicsRotator.MinLimitReached += delegate
            {
                startText.color = Color.green;
                _startIsSelected = true;
            };
            
            _physicsRotator.MinLimitExited += delegate
            {
                startText.color = Color.white;
                _startIsSelected = false;
            };
            
            _physicsRotator.MaxLimitReached += delegate
            {
                resetText.color = Color.green;
                _resetIsSelected = true;
            };
            
            _physicsRotator.MaxLimitExited += delegate 
            {
                resetText.color = Color.white;
                _resetIsSelected = false;
            };

            _interactableObject.InteractableObjectGrabbed += delegate
            {
                resetText.color = Color.white;
                startText.color = Color.white;
                
                resetText.enabled = true;
                startText.enabled = true;

                _resetIsSelected = false;
                _startIsSelected = false;
            };
            
            _interactableObject.InteractableObjectUngrabbed += delegate
            {
                resetText.enabled = false;
                startText.enabled = false;
                
                if (_resetIsSelected)
                {
                    level.ResetLevel();
                }
                else if (_startIsSelected)
                {
                    level.StartLevel();
                }
                
                _physicsRotator.SetValue(0);
            };
        }
    }
}