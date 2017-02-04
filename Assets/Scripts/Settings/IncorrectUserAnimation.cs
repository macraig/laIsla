using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Settings
{

    public class IncorrectUserAnimation : MonoBehaviour
    {

        public SwitchPlayerView switchPlayerView;

        public void OnIncorrectUserAnimationEnd()
        {
            gameObject.GetComponent<Animation>().Stop();
            gameObject.SetActive(false);
            switchPlayerView.OnIncorrectInputAnimationEnd();
        }

        internal void ShowIncorrecrUserAnimation()
        {
            gameObject.gameObject.SetActive(true);
            gameObject.GetComponent<Animation>().Play();

        }

    }
}
