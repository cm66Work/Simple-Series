using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.ES.SimpleSystems.Demos
{
    public class Menu : MonoBehaviour
    {
        [Header("First Selected Button")]
        [SerializeField] private GameObject m_firstSelected;

        protected virtual void OnEnable()
        {
            StartCoroutine(SetFirstSelected(m_firstSelected));
        }

        public IEnumerator SetFirstSelected(GameObject firstSelected)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}
