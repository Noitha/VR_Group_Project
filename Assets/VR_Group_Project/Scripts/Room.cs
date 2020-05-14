using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class Room : MonoBehaviour
    {
        public string roomName;
        
        //bind object to their rooms.
        
        public List<Tool> tools = new List<Tool>();
        
        /// <summary>
        /// All the levels in a room.
        /// </summary>
        /// 
        private List<Level> _levels = new List<Level>();

        public void Initialize()
        {
            tools = GetComponentsInChildren<Tool>().ToList();
            _levels = GetComponentsInChildren<Level>().ToList();
            
            
            foreach (var tool in tools)
            {
                tool.Initialize(null);
            }
            
            foreach (var level in _levels)
            {
                level.Initialize();
            }
        }
    }
}