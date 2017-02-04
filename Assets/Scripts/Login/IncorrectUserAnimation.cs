using UnityEngine;

namespace Assets.Scripts.Login
{

    public class IncorrectUserAnimation : MonoBehaviour {

        public LoginView loginView;

        public void OnIncorrectUserAnimationEnd()
        {
            gameObject.SetActive(false);
            loginView.OnIncorrectInputAnimationEnd();
        }

        internal void ShowIncorrecrUserAnimation()
        {
            gameObject.SetActive(true);

        }

    }

}
