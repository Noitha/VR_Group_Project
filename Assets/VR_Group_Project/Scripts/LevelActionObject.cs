using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public abstract class LevelActionObject : BaseLevelObject
    {
        protected Rigidbody objectRigidbody;
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);

            tag = "LevelActionObject";
            gameObject.layer = 14;
            objectRigidbody = GetComponent<Rigidbody>();
        }
        
        public abstract void Action(BaseUnit selectedAnimals);
    }
}