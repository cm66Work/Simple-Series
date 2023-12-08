using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.ES.SimpleSystems.Demos
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button m_newGameButton;
        [SerializeField] private Button m_continueGameButton;

        private void Start()
        {
            if (false == SaveSystem.SimpleSaveSystemCore.instance.HasGameData())
                m_continueGameButton.interactable = false;
        }

        public void OnNewGameClicked()
        {
            DisableAllButtons();

            // Create a new game - which will initialize our game data.
            SimpleSystems.SaveSystem.SimpleSaveSystemCore.instance.NewGame();
            // load the gameplay scene - which will in turn save the game because of
            // OnSceneUnloaded() in the SimpleSaveSystemCore.
            SceneManager.LoadSceneAsync("Gameplay");
        }

        public void OnContinueClicked()
        {
            DisableAllButtons();

            // load the next scene - which will in turn load the game because of
            // OnSceneLoaded() in SimpleSaveSystemCore.
            SceneManager.LoadSceneAsync("Gameplay");
        }

        private void DisableAllButtons()
        {
            m_newGameButton.interactable = false;
            m_continueGameButton.interactable = false;
        }
    }
}
