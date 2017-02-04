using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Sound;
using Assets.Scripts.Settings;

namespace Assets.Scripts.Login{

    public class LoginView : MonoBehaviour{

        //todo -> get var from Settings
        public InputField inputText;
        public Text incorrectInput;
        public Text title;
        public Button ticBtn;

        // Use this for initialization
        void OnEnable(){
            incorrectInput.gameObject.SetActive(false);
        }

        void Update(){
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) { CheckEnteredUsername(); }
            else if(Input.GetKeyDown(KeyCode.Escape)) OnClickBack();
        }

       

        public void OnClickTicBtn(){
            PlayClickSound();
            CheckEnteredUsername();
        }

        void CheckEnteredUsername(){
            inputText.text = inputText.text.Trim();
            LoginController.GetController().SaveUsername(inputText.text.ToLower());
        }

        internal void ShowIncorrectInputAnimation(){
            ticBtn.interactable = false;
            ticBtn.enabled = false;
            incorrectInput.GetComponent<IncorrectUserAnimation>().ShowIncorrecrUserAnimation();
        }

        public void OnIncorrectInputAnimationEnd(){          
            ticBtn.interactable = true;
            ticBtn.enabled = true;
        }

        public void OnClickBack(){
            PlayClickSound();
            LoginController.GetController().GoBack();
        }

        public void PlayClickSound(){
            SoundController.GetController().PlayClickSound();
        }

       
    }
}