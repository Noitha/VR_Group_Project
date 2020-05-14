using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public abstract class BaseLevelObject : MonoBehaviour
    {
        protected Level Level;

        public virtual void Initialize(Level level)
        {
            Level = level;

            GetComponents();
            InitializeComponents();
        }

        protected virtual void GetComponents() { }
        protected virtual void InitializeComponents() { }
    }
}