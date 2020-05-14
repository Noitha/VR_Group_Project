using System;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class MovingPlatform : BaseLevelObject
    {
        public Transform startTransform;
        public Transform endTransform;
        public float speed;
        
        public PressurePlate pressurePlateA;
        public PressurePlate pressurePlateB;

        public TriggerController triggerController;
        public Transform movingPlatform;

        public List<BaseUnit> unitsOnPlatform = new List<BaseUnit>();
        private AudioSource _audioSource;
        public AudioClip moveSound;

        private float travelLength;
        public float _progress;

        public GameObject frontObstacle;
        public GameObject endObstacle;
        
        
        //Maybe convert to a coroutine, while active
        private void Update()
        {
            if (!pressurePlateA.IsActive && !pressurePlateB.IsActive)
            {
                return;
            }
            
            switch (_progress)
            {
                case 0:
                    frontObstacle.SetActive(true);
                    endObstacle.SetActive(false);
                    break;
                case 1:
                    frontObstacle.SetActive(false);
                    endObstacle.SetActive(true);
                    break;
            }
            
            movingPlatform.position = Vector3.MoveTowards
            (
                movingPlatform.position,
                pressurePlateA.IsActive ? endTransform.position : startTransform.position,
                speed * Time.deltaTime
            );
            
            var distanceFromStart = Vector3.Distance(movingPlatform.position, startTransform.position);
            var distanceFromEnd = Vector3.Distance(movingPlatform.position, endTransform.position);

            _progress = Mathf.InverseLerp
            (
                pressurePlateA.IsActive ? 0 : travelLength,
                pressurePlateA.IsActive ? travelLength : 0,
                pressurePlateA.IsActive ? distanceFromEnd : distanceFromStart
            );
            
            foreach (var unit in unitsOnPlatform)
            {
                unit.transform.position = Vector3.MoveTowards
                (
                    unit.transform.position,
                    pressurePlateA.IsActive ? endTransform.position : startTransform.position,
                    speed * Time.deltaTime
                );
            }
            
            movingPlatform.eulerAngles = Vector3.RotateTowards
            (
                movingPlatform.eulerAngles,
                pressurePlateA.IsActive ? endTransform.eulerAngles : startTransform.eulerAngles,
                speed * Time.deltaTime,
                2
            );
            
            if (!(Vector3.Distance(movingPlatform.position, pressurePlateA.IsActive ? endTransform.position : startTransform.position) > 0.0025f))
            {
                _audioSource.Stop();
                return;
            }

            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }

        private void Lock()
        {
            foreach (var unit in unitsOnPlatform)
            {
                unit.StopAgent();
                unit.DisableAgent();
            }
        }

        private void UnLock()
        {
            if (Math.Abs(_progress) > .01f && Math.Abs(_progress - 1) > .01f)
            {
                return;
            }
            
            Level.RebuildSurfaceMesh();
            
            foreach (var unit in unitsOnPlatform)
            {
                unit.EnableAgent();
                unit.StartAgent();
            }
        }

        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            travelLength = Vector3.Distance(startTransform.position, endTransform.position);

            movingPlatform.gameObject.layer = 12;
            
            _audioSource = GetComponent<AudioSource>();
            
            _audioSource.clip = moveSound;
            
            pressurePlateA.UnLock();
            pressurePlateB.UnLock();
            UnLock();
            
            triggerController.onObjectTriggerEnter += delegate(GameObject obj)
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }

                unitsOnPlatform.Add(unit);
            };
            
            triggerController.onObjectTriggerExit += delegate(GameObject obj)
            {
                var unit = obj.GetComponent<BaseUnit>();

                if (unit == null)
                {
                    return;
                }

                unitsOnPlatform.Remove(unit);
            };
            
            pressurePlateA.onPlatformActivate += delegate
            {
                pressurePlateB.Lock();
                Lock();
            };
            
            pressurePlateB.onPlatformActivate += delegate
            {
                pressurePlateA.Lock();
                Lock();
            };
            
            pressurePlateB.onPlatformDesActivate += delegate
            {
                pressurePlateA.UnLock();
                UnLock();
            };
            
            pressurePlateA.onPlatformDesActivate += delegate
            {
                pressurePlateB.UnLock();
                UnLock();
            };
        }
    }
}