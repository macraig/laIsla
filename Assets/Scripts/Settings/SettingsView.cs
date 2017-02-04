using Assets.Scripts.App;
using System;
using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Settings
{

    public class SettingsView : MonoBehaviour
    {
        public SwitchPlayerView switchPlayerView;
        public GeneralSettingsView generalSettingsView;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(switchPlayerView.isActiveAndEnabled) switchPlayerView.OnClickCrossBtn();
                else generalSettingsView.OnClickClose();
            }
        }

        internal void ShowSwitchPlayer()
        {
            switchPlayerView.gameObject.SetActive(true);
        }

        internal void CloseSettings()
        {
            ViewController.GetController().LoadMainMenu();
        }

        internal void ShowGeneralSettings()
        {
            generalSettingsView.gameObject.SetActive(true);
        }
    }
}
