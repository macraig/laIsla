using Assets.Scripts.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cover
{

    public class AboutScreen : MonoBehaviour
    {
        public CoverView coverView;

        public Text nameLabel;
        public Text ageRangeLabel;
        public Text areaLabel;
        public Text areaText;
        public Text contentsLabel;
        public Text contentsText;
        public Text creditsLabel;
		    

        public void OnClickAboutScreen(){
            coverView.ClickSound();
            coverView.ShowCoverScreen();
            gameObject.SetActive(false);
        }

        
    }
}