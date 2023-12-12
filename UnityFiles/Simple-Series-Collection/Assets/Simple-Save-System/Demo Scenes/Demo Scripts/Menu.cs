using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace com.ES.SimpleSystems.Demos
{
    public class Menu : MonoBehaviour
    {
        [Header("First Selected Button")]
        [SerializeField] private Button m_firstSelected;

        protected virtual void OnEnable()
        {
            SetFirstSelected(m_firstSelected);
        }

        public void SetFirstSelected(Button firstSelected)
        {
            firstSelected.Select(); 
        }
    }
}
