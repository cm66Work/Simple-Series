using com.ES.SimpleSystems.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.ES.SimpleSystems.Demos
{
    public class SaveSlot : MonoBehaviour
    {
        [Header("Profile ID")][SerializeField] private string m_profileID = "";
        public string GetProfileID() => m_profileID;

        [Header("Content")]
        [SerializeField] private GameObject m_noDataContent;
        [SerializeField] private GameObject m_hadDataContent;
        [SerializeField] private TextMeshProUGUI m_money;
        [SerializeField] private TextMeshProUGUI m_saveSlotName;

        private Button m_saveSlotButton;

        private void Awake()
        {
            m_saveSlotButton = this.GetComponent<Button>();
        }

        public void SetData(GameData data)
        {
            // there is no data for this profileID
            if(null == data)
            {
                m_noDataContent.SetActive(true);
                m_hadDataContent.SetActive(false);
            }
            else // there is data for this profileID
            {
                m_noDataContent.SetActive(false);
                m_hadDataContent.SetActive(true);

                m_money.text = "MONEY: " + data.exampleInt;
                m_saveSlotName.text = "SAVE SLOT: " + m_profileID;
            }
        }

        public void SetInteractable(bool interactable)
        {
            m_saveSlotButton.interactable = interactable;
        }
    }
}
