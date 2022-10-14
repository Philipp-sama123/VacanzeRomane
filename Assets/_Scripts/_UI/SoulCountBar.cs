using UnityEngine;
using UnityEngine.UI;

namespace _Scripts._UI {
    public class SoulCountBar : MonoBehaviour {
        public Text soulCountText;

        public void SetSoulCountText(int soulCount)
        {
            soulCountText.text = soulCount.ToString();
        }
    }
}