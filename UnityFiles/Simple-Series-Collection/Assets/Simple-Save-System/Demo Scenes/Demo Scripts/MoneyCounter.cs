using com.ES.SimpleSystems.SaveSystem;
using TMPro;
using UnityEngine;

namespace com.ES.SimpleSystems.Demos
{
    public class MoneyCounter : MonoBehaviour, IDataPersistence
    {
        private int m_money = 0;

        public void LoadData(GameData data)
        {
            this.m_money = data.exampleInt;
            m_moneyTMP.text = this.m_money.ToString();
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
            m_moneyTMP.text = m_money.ToString();
        }


        private void Awake()
        {
            m_moneyTMP = this.GetComponent<TMP_Text>();
        }
        #endregion
    }
}
