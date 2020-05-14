using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VR_Group_Project.Scripts
{
    public class TreeFallAction : LevelActionObject
    {
        [Header("Setup")]
        public PlaceableNavigation placeableNavigation;
        public int hitAmount;
        public ToolType requiredTool;
        public bool requiresHand;

        private AudioSource _audioSource;
        public AudioClip treeFallClip;
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);

            _audioSource = GetComponent<AudioSource>();
            objectRigidbody.useGravity = false;
        }

        public override void Action(BaseUnit selectedAnimal)
        {
            if (selectedAnimal.animalType != AnimalType.Boar)
            {
                return;
            }

            _audioSource.clip = treeFallClip;
            _audioSource.Play();
            
            var newTransformableObject = gameObject.AddComponent<TransformableObject>();
            newTransformableObject.Initialize(Level);
                
            newTransformableObject.Setup(placeableNavigation, hitAmount, requiredTool, requiresHand);
            Destroy(GetComponent<TreeFallAction>());
            StartCoroutine(BreakTree(selectedAnimal.unitCamera.transform.forward));
        }


        private IEnumerator BreakTree(Vector3 direction)
        {
            objectRigidbody.useGravity = true;
            objectRigidbody.AddForce(direction);
            
            yield return new WaitUntil(() =>
            {
                objectRigidbody.velocity *= .95f * Time.deltaTime;

                return Mathf.Abs(objectRigidbody.velocity.sqrMagnitude) < .1;
            });
        }
    }
}