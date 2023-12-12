using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace com.ES.SimpleSystems.Demos
{
    public class ConfirmationPopupMenu : Menu 
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI m_displayText;
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Button m_cancelButton;

        public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
        {
            this.gameObject.SetActive(true);

            // set the display text
            this.m_displayText.text = displayText;

            // remove any existing listeners just to make sure there are not any previous ones hanging around.
            // note - this only removes listeners added through code!
            m_confirmButton.onClick.RemoveAllListeners();
            m_cancelButton.onClick.RemoveAllListeners();

            // assign the onClick listeners
            m_confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                confirmAction();
            });
            m_cancelButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                cancelAction();
            });
        }

        private void DeactivateMenu()
        {
            this.gameObject.SetActive(false);
        }
    }
}
