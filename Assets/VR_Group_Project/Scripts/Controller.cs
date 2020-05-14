using UnityEngine;
using VRTK;

namespace VR_Group_Project.Scripts
{
    public class Controller : MonoBehaviour
    {
        private VRTK_ControllerEvents _controllerEvents;
        private VRTK_InteractGrab _interactGrab;
        private TriggerController _triggerController;
        
        public Hand hand;
        public Level level;
        public Player player;
        
        public void Initialize(Player p, Hand h)
        {
            player = p;
            hand = h;
            
            _controllerEvents = GetComponent<VRTK_ControllerEvents>();
            _interactGrab = GetComponent<VRTK_InteractGrab>();
            _triggerController = GetComponent<TriggerController>();
            
            _triggerController.onObjectTriggerEnter += delegate(GameObject obj)
            {
                if (!obj.CompareTag("Level"))
                {
                    return;
                }
                
                level = obj.GetComponent<Level>();
                player.DetermineLevel();
            };
            
            _triggerController.onObjectTriggerExit += delegate(GameObject obj)
            {
                if (!obj.CompareTag("Level"))
                {
                    return;
                }
                
                level = null;
                player.DetermineLevel();
            };
        }

        public VRTK_ControllerEvents GetControllerEvents()
        {
            return _controllerEvents;
        }

        public VRTK_InteractGrab GetInteractGrab()
        {
            return _interactGrab;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void UnGrab()
        {
            player.UnGrab(hand);
        }
    }
}