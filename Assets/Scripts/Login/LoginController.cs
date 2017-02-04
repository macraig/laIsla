using UnityEngine;
using Assets.Scripts.Settings;
using Assets.Scripts.App;
using Assets.Scripts.Sound;

namespace Assets.Scripts.Login
{

    public class LoginController : MonoBehaviour
    {
        private static LoginController loginController;
        public LoginView loginView;

        void Awake(){
            if (loginController == null){
                loginController = this;
            }else if (loginController != this){
                Destroy(gameObject);
            }
        }

        void Start()
        {
            loginView.inputText.onValueChanged.AddListener(delegate
            {
                SoundController.GetController().PlayTypingSound();
            });
        }

        public void SaveUsername(string username){
            if(username != "" && username.Length > 2){
                SettingsController.GetController().SwitchName(username);
                ViewController.GetController().LoadMainMenu();
            } else{
                loginView.ShowIncorrectInputAnimation();
            }

        }

        internal void GoBack() {
            ViewController.GetController().LoadCover();
        }

        public static LoginController GetController(){
            return loginController;
        }
    }
}
