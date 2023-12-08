using System;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    /// <summary>
    /// Gets placed on Game Object that needs to have its data saved.
    /// </summary>
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string m_ID = string.Empty;

        public string ID => m_ID;

        [ContextMenu("Generate ID")]
        private void GenerateID() => m_ID = Guid.NewGuid().ToString();
    }
}
