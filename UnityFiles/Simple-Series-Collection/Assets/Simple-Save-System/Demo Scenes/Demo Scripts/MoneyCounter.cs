using TMPro;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class MoneyCounter : MonoBehaviour, IDataPersistence
    {
        private int m_money = 0;

        public void LoadData(GameData data)
        {
            this.m_money = data.exampleInt;
        }

        public void SaveData(ref GameData data)
        {
            data.exampleInt = this.m_money;
        }




        #region Not Important
        [SerializeField] private TMP_Text m_moneyTMP;

        public void AddMoney()
        {
            m_money += 10;
            if(m_moneyTMP == null)
            {
                m_moneyTMP = this.GetComponent<TMP_Text>();
            }

            m_moneyTMP.text = m_money.ToString();
        }
        #endregion
    }
}
