using TMPro;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class LevelDestination : BaseLevelObject
    {
        private TriggerController _triggerController;
        private AudioSource _audioSource;
        public AudioClip zone;
        
        private int _amountOfUnitsInsideDestinationBoundaries;
        public TextMeshProUGUI amountOfUnitsInsideDestinationBoundariesTextBack;
        public TextMeshProUGUI amountOfUnitsInsideDestinationBoundariesTextFront;
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            _triggerController = GetComponent<TriggerController>();
            _audioSource = GetComponent<AudioSource>();

            amountOfUnitsInsideDestinationBoundariesTextBack.text = "0/" + level.UnitsInLevel.Count;
            amountOfUnitsInsideDestinationBoundariesTextFront.text = "0/" + level.UnitsInLevel.Count;
            
            _triggerController.onObjectTriggerEnter += delegate(GameObject obj) 
            {
                if (!obj.CompareTag("Unit"))
                {
                    return;
                }

                _audioSource.PlayOneShot(zone);
                _amountOfUnitsInsideDestinationBoundaries++;
                amountOfUnitsInsideDestinationBoundariesTextBack.text = _amountOfUnitsInsideDestinationBoundaries + "/" + level.UnitsInLevel.Count;
                amountOfUnitsInsideDestinationBoundariesTextFront.text = _amountOfUnitsInsideDestinationBoundaries + "/" + level.UnitsInLevel.Count;
                level.CheckLevelCompletion(_amountOfUnitsInsideDestinationBoundaries);
            };
            
            _triggerController.onObjectTriggerExit += delegate(GameObject obj) 
            {
                if (!obj.CompareTag("Unit"))
                {
                    return;
                }

                _audioSource.PlayOneShot(zone);
                _amountOfUnitsInsideDestinationBoundaries--;
                amountOfUnitsInsideDestinationBoundariesTextBack.text = _amountOfUnitsInsideDestinationBoundaries + "/" + level.UnitsInLevel.Count;
                amountOfUnitsInsideDestinationBoundariesTextFront.text = _amountOfUnitsInsideDestinationBoundaries + "/" + level.UnitsInLevel.Count;
                level.CheckLevelCompletion(_amountOfUnitsInsideDestinationBoundaries);
            };
        }
    }
}