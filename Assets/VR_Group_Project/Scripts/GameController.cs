using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// Singleton Instance.
        /// </summary>
        public static GameController Instance;

        public Player player;
        
       /// <summary>
       /// All the rooms.
       /// </summary>
        private List<Room> _rooms = new List<Room>();
        

        public const float CheckPositionChangeRepeatTime = .5f;
        public const float RecalculatePathDistanceThreshold = .2f;
        
        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            tag = "GameController";
            
            GetComponents();
            InitializeComponents();
        }

        #endregion
        
        private void GetComponents()
        {
            _rooms = FindObjectsOfType<Room>().ToList();
        }
        
        private void InitializeComponents()
        {
            player.Initialize();
            
            Level.SetValidHitIndicator(Resources.Load<GameObject>("Prefabs/Level/Hit Indicators/Valid"));
            Level.SetInValidHitIndicator(Resources.Load<GameObject>("Prefabs/Level/Hit Indicators/Invalid"));

            foreach (var room in _rooms)
            {
                room.Initialize();
            }
        }
    }
}