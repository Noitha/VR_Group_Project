using System;
using System.Collections;

using UnityEngine;

namespace VR_Group_Project.Scripts
{
    [Serializable]
    public class Dialogue : BaseSequence
    {
        public BaseUnit baseUnit;
        public string dialogue;

        public override IEnumerator Display(Level level)
        {
            baseUnit.ClearDialogue();

            foreach (var character in dialogue)
            {
                switch (character)
                {
                    case '.':
                        yield return new WaitForSeconds(.6f);
                        break;
                    
                    case ',':
                        yield return new WaitForSeconds(.3f);
                        break;
                    
                    default:
                        yield return new WaitForSeconds(.05f);
                        break;
                }

                baseUnit.AppendCharacterToDialogue(character);
            }
            
            level.NextDialogue();
        }
    }
}