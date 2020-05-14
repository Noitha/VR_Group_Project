using System;
using System.Collections;

namespace VR_Group_Project.Scripts
{
    [Serializable]
    public abstract class BaseSequence
    {
        public abstract IEnumerator Display(Level level);
    }
}