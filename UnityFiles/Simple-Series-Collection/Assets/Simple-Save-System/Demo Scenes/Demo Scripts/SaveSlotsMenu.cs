using com.ES.SimpleSystems.SaveSystem;
using System;
using System.Collections;
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

            // update the selected profile id to be used for data persistence.
            SimpleSaveSystemCore.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

            if(!m_isLoadingGame)
            {
                // create a new game - which will initialize our data to a clean slate.
                SimpleSaveSystemCore.instance.NewGame();
            }

            // load the scene - which will in turn save the game because of OnSceneUnload() in SimpleSaveSystemCore.
            SceneManager.LoadSceneAsync("Gameplay");
        }

        public void ActivateMenu(bool isLoadingGame)
        {
            // set this menu to be active.
            this.gameObject.SetActive(true);

            // set mode
            this.m_isLoadingGame = isLoadingGame;

            // load all of the profiles that exist
            Dictionary<string, GameData> profilesGameData = SimpleSaveSystemCore.instance.GetAllProfilesGameData();

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
            StartCoroutine(this.SetFirstSelected(firstSelected));
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
