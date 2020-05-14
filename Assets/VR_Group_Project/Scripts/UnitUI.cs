using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VR_Group_Project.Scripts
{
    public class UnitUI : MonoBehaviour
    {
        public TextMeshProUGUI dialogueTextBack;
        public TextMeshProUGUI dialogueTextFront;
        
        public Image unitSelectedImage;
        
        public Color unitSelected;

        public void Select()
        {
            unitSelectedImage.color = new Color(unitSelected.r, unitSelected.g, unitSelected.b, 255);
        }
        
        public void DeSelect()
        {
            unitSelectedImage.color = new Color(unitSelected.r, unitSelected.g, unitSelected.b, 0);
        }
    }
}