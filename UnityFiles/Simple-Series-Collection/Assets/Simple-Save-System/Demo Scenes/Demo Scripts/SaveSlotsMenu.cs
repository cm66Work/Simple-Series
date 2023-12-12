using com.ES.SimpleSystems.SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.ES.SimpleSystems.Demos
{
    public class SaveSlotsMenu : Menu 
    {
        [Header("Menu Navigation")]
        [SerializeField] private MainMenu m_mainMenu;

        [Header("Menu Buttons")]
        [SerializeField] private Button m_backButton;

        [Header("Confirmation Popup")]
        [SerializeField] private ConfirmationPopupMenu m_confirmationPopup;

        private SaveSlot[] m_saveSlots;

        private bool m_isLoadingGame = false;

        private void Awake()
        {
            m_saveSlots = GetComponentsInChildren<SaveSlot>();
        }

        public void OnBackClicked()
        {
            m_mainMenu.ActivateMenu();
            DeactivateMenu();
        }

        public void OnSaveSlotClicked(SaveSlot saveSlot)
        {
            // disable all buttons.
            DisableAllButtons();

            // case - loading game
            if(m_isLoadingGame)
            {
                SimpleSaveSystemCore.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
                SaveGameAndLoadScene();
            }
            // case - new game, but the save slot has data
            else if(saveSlot.hasData)
            {
                m_confirmationPopup.ActivateMenu(
                    $"Starting a new Game with this slot will override the currently saved data. \n" +
                    $"Are you sure?",
                    // function to execute if we select "Yes"
                    () =>
                    {
                        SimpleSaveSystemCore.instance.ChangeSelectedProfileID(saveSlot.GetProfileID() );
                        SimpleSaveSystemCore.instance.NewGame();
                        SaveGameAndLoadScene();
                    },
                    // function to execute if we select "Cancel"
                    () =>
                    {
                        this.ActivateMenu(m_isLoadingGame);
                    }
                    );
            }
            // case - new game, and the save slot had no data.
            else
            {
                SimpleSaveSystemCore.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
                SimpleSaveSystemCore.instance.NewGame();
                SaveGameAndLoadScene();
            }
        }

        private void SaveGameAndLoadScene()
        {
            // save the game anytime BEFORE loading a new scene.
            SimpleSaveSystemCore.instance.SaveGame();

            // load the scene - which will in turn save the game because of OnSceneUnload() in SimpleSaveSystemCore.
            SceneManager.LoadSceneAsync("Gameplay");
        }

        public void OnClearClicked(SaveSlot saveSlot)
        {
            DisableAllButtons();

            m_confirmationPopup.ActivateMenu(
                "Are you sure you want to delete this save?",
                // function to execute if we select "Yes".
                () =>
                {
                    SimpleSaveSystemCore.instance.DeleteProfileData(saveSlot.GetProfileID());
                    ActivateMenu(m_isLoadingGame);
                },
                // function to execute if we select "Cancel".
                () =>
                {
                    ActivateMenu(m_isLoadingGame);
                }
                );

        }

        public void ActivateMenu(bool isLoadingGame)
        {
            // set this menu to be active.
            this.gameObject.SetActive(true);

            // set mode
            this.m_isLoadingGame = isLoadingGame;

            // load all of the profiles that exist
            Dictionary<string, GameData> profilesGameData = SimpleSaveSystemCore.instance.GetAllProfilesGameData();

            // ensure the back button is enabled when we activate the menu.
            m_backButton.interactable = true;
            // loop through each save slot in the UI and set the content appropriately.
            GameObject firstSelected = m_backButton.gameObject;
            foreach (SaveSlot saveSlot in m_saveSlots)
            {
                GameData profileData = null;
                profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
                saveSlot.SetData(profileData); // SetData will handle the null check. 
                if(null == profileData && m_isLoadingGame)
                {
                    saveSlot.SetInteractable(false);
                }
                else
                {
                    saveSlot.SetInteractable(true);
                    if (firstSelected.Equals(m_backButton.gameObject))
                        firstSelected = saveSlot.gameObject;
                }
            }

            // set the first selected button.
            Button firstSelectedButton = firstSelected.GetComponent<Button>();
            SetFirstSelected(firstSelectedButton);
        }

        public void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }

        private void DisableAllButtons()
        {
            foreach (SaveSlot saveSlot in m_saveSlots)
            {
                saveSlot.SetInteractable(false);
            }
            m_backButton.interactable = false;
        }
    }
}
