using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    [RequireComponent(typeof(TerrainNavigableSurface), typeof(AudioSource), typeof(BoxCollider))]
    public class Level : MonoBehaviour
    {
        /// <summary>
        /// The navigable area of the terrain.
        /// </summary>
        private TerrainNavigableSurface _terrainNavigableSurface;

        //Audio
        /// <summary>
        /// Audio source for the level.
        /// </summary>
        private AudioSource _audioSource;

        //Create a sound controller and extract those out.
        
        private static AudioClip validHitPosition;
        private static AudioClip invalidHitPosition;
        private static AudioClip winSound;
        
        public List<LevelResetData> levelResetData = new List<LevelResetData>();
        
        /// <summary>
        /// List of all the objects that are level depended.
        /// </summary>
        private List<BaseLevelObject> _levelObjects = new List<BaseLevelObject>();
        
        /// <summary>
        /// List of all the units in the level.
        /// </summary>
        public List<BaseUnit> UnitsInLevel { get; private set; } = new List<BaseUnit>();
        
        /// <summary>
        /// List of dialogues.
        /// </summary>
        public List<Dialogue> dialogues = new List<Dialogue>();

        /// <summary>
        /// Current track of the dialogue.
        /// </summary>
        private int _currentTrackOfDialogue;
        
        private static GameObject validHitIndicator;
        private static GameObject invalidHitIndicator;
        
        public Transform placeableNavigationObjectsContainer;

        private bool _completed;
        
        public void Initialize()
        {
            GetComponents();
            InitializeComponents();
        }
        
        private void GetComponents()
        {
            _terrainNavigableSurface = GetComponent<TerrainNavigableSurface>();
            _levelObjects = GetComponentsInChildren<BaseLevelObject>().ToList();
            UnitsInLevel = GetComponentsInChildren<BaseUnit>().ToList();
            _audioSource = GetComponent<AudioSource>();
        }
        
        private void InitializeComponents()
        {
            //Initialize all the level objects.
            foreach (var levelObject in _levelObjects)
            {
                levelObject.Initialize(this);
            }

            foreach (var unit in UnitsInLevel)
            {
                unit.enabled = false;
            }
            
            _audioSource.minDistance = 2;
            _audioSource.maxDistance = 3;
        }
        
        
        public void StartLevel()
        {
            StartDialogue();
        }

        private void StartDialogue()
        {
            if (dialogues.Count == 0)
            {
                return;
            }
            
            _currentTrackOfDialogue = 0;
            StartCoroutine(dialogues[_currentTrackOfDialogue].Display(this));
        }
        
        public void NextDialogue()
        {
            if (_currentTrackOfDialogue >= dialogues.Count - 1)
            {
                return;
            }
            
            _currentTrackOfDialogue++;
            StartCoroutine(dialogues[_currentTrackOfDialogue].Display(this));
        }
        
        public static void SetValidHitIndicator(GameObject obj)
        {
            validHitIndicator = obj;
        }
        
        public static void SetInValidHitIndicator(GameObject obj)
        {
            invalidHitIndicator = obj;
        }

        public void CheckLevelCompletion(int amount)
        {
            if (UnitsInLevel.Count == amount)
            {
                _audioSource.PlayOneShot(winSound);
            }
            
            //Win mark as it.
        }

        public void RebuildSurfaceMesh()
        {
            _terrainNavigableSurface.RebuildSurface();
        }

        public IEnumerator RebuildNavMeshAfterDelay()
        {
            yield return new WaitForSeconds(1);
            _terrainNavigableSurface.RebuildSurface();
        }

        public void ResetLevel()
        {
            //Turn Left to restart level, turn right to start level.
            
            
            /* foreach (var unitResetData in levelResetData.unitResetData)
             {
                 unitResetData.unit.navMeshAgent.isStopped = true;
                 unitResetData.unit.navMeshAgent.enabled = false;
                 
                 var levelObjectTransform = unitResetData.unit.transform;
                 levelObjectTransform.position = unitResetData.unitPosition;
                 levelObjectTransform.eulerAngles = unitResetData.unitRotation;
                 
                 unitResetData.unit.navMeshAgent.enabled = true;
             }
             
             foreach (var objectResetData in levelResetData.objectResetData)
             {
                 var levelObjectTransform = objectResetData.levelObject.transform;
                 levelObjectTransform.position = objectResetData.objectPosition;
                 levelObjectTransform.eulerAngles = objectResetData.objectRotation;
             }*/
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            var player = other.GetComponentInParent<Player>();
            player.isInsideLevel = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            var player = other.GetComponentInParent<Player>();
            player.isInsideLevel = false;
        }
    }
}