using System.Collections;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class Teleportation : BaseLevelObject
    {
        public Transform transformA;
        public Transform transformB;

        public TriggerController triggerControllerA;
        public TriggerController triggerControllerB;

        public Animator animatorA;
        public Animator animatorB;
      
        public AnimalType allowedAnimal;
        
       
        private const float timer = 3f;
        private Coroutine _currentTeleport;
        private static readonly int ElapsedTime = Animator.StringToHash("ElapsedTime");


        private bool _teleportHasStarted;
        private bool _teleportIsComplete;
        private TriggerController _teleportPointUsed;
        
        
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            _currentTeleport = null;
            
            animatorA.SetFloat(ElapsedTime, 0);
            animatorB.SetFloat(ElapsedTime, 0);
            
            animatorA.GetComponent<MeshRenderer>().enabled = false;
            animatorB.GetComponent<MeshRenderer>().enabled = false;

            animatorA.gameObject.isStatic = false;
            animatorB.gameObject.isStatic = false;
            
            triggerControllerA.onObjectTriggerEnter += delegate(GameObject obj)
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }
                
                if (unit.animalType != allowedAnimal)
                {
                    return;
                }

                if (_currentTeleport != null)
                {
                    return;
                }
                
                _teleportPointUsed = triggerControllerA;
                _teleportHasStarted = true;
                _teleportIsComplete = false;
                _currentTeleport = StartCoroutine(Teleport(animatorA, unit, transformB.position));
            };
            
            triggerControllerB.onObjectTriggerEnter += delegate(GameObject obj) 
            { 
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }
                
                if (unit.animalType != allowedAnimal)
                {
                    return;
                }

                if (_currentTeleport != null)
                {
                    return;
                }
                
                _teleportPointUsed = triggerControllerB;
                _teleportHasStarted = true;
                _teleportIsComplete = false;
                _currentTeleport = StartCoroutine(Teleport(animatorB, unit, transformA.position));
            };
            
            triggerControllerA.onObjectTriggerExit += delegate(GameObject obj) 
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }
                
                if (unit.animalType != allowedAnimal)
                {
                    return;
                }
                
                if (_teleportIsComplete)
                {
                    if (_teleportPointUsed != triggerControllerA)
                    {
                        _currentTeleport = null;
                    }
                }
                else
                {
                    StopCoroutine(_currentTeleport);
                    _currentTeleport = null;
                    animatorA.SetFloat(ElapsedTime, 0);
                }
            };
            
            triggerControllerB.onObjectTriggerExit += delegate(GameObject obj) 
            { 
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }
                
                if (unit.animalType != allowedAnimal)
                {
                    return;
                }
                
                if (_teleportIsComplete)
                {
                    if (_teleportPointUsed != triggerControllerB)
                    {
                        _currentTeleport = null;
                    }
                }
                else
                {
                    StopCoroutine(_currentTeleport);
                    _currentTeleport = null;
                    animatorB.SetFloat(ElapsedTime, 0);
                }
            };
        }
        private IEnumerator Teleport(Animator animator, BaseUnit unit, Vector3 position)
        {
            var elapsedTime = 0f;

            yield return new WaitUntil(delegate
            {
                if (elapsedTime > timer)
                {
                    _teleportIsComplete = true;
                    unit.DisableAgent();
                    unit.transform.position = position;
                    unit.EnableAgent();
                    animator.SetFloat(ElapsedTime, 0);
                    animator.GetComponent<MeshRenderer>().enabled = false;
                    return true;
                }

                elapsedTime += Time.deltaTime;
                animator.SetFloat(ElapsedTime, elapsedTime);
                animator.GetComponent<MeshRenderer>().enabled = true;
                return false;
            });
        }
    }
}