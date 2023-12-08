using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.ES.SimpleSystems.Demos
{
    public class MainMenu : Menu 
    {
        [Header("Menu Navigation")]
        [SerializeField] private SaveSlotsMenu m_saveSlotsMenu;
        [SerializeField] private GameObject m_firstSelectedMenuObject;

        [Header("Menu Buttons")]
        [SerializeField] private Button m_newGameButton;
        [SerializeField] private Button m_continueGameButton;
        [SerializeField] private Button m_loadGameButton;

        private void Start()
        {
            if (false == SaveSystem.SimpleSaveSystemCore.instance.HasGameData())
            {
                m_continueGameButton.interactable = false;
                m_loadGameButton.interactable = false;
            }
        }

        public void OnNewGameClicked()
        {
            m_saveSlotsMenu.ActivateMenu(false);
            this.DeactivateMenu();
        }

        public void OnLoadGameClicked()
        {
            m_saveSlotsMenu.ActivateMenu(true);
            this.DeactivateMenu();
        }

        public void ActivateMenu()
        {
            this.gameObject.SetActive(true);
        }

        private void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
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
