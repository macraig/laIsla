using UnityEngine;
using Assets.Scripts.App;
using UnityEngine.UI;

namespace Assets.Scripts.Levels
{
    public class SpeakerScript : MonoBehaviour
    {
        public static SpeakerScript instance;
        public Animator animator;
		 public Button soundButton;

        void Awake() {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(this);
        }

        // Use this for initialization
        void Start() { 
        }        

        public void PlaySound(int digits)
        {
//			if (soundButton) {
				 soundButton.enabled = false;
				animator.SetBool("isPlayingSound", true);        
			Invoke("silence", digits * 0.6f); 
//			}         
        }

        void silence()
        {
			 soundButton.enabled = true;
           animator.SetBool("isPlayingSound", false);
        }
    }
}