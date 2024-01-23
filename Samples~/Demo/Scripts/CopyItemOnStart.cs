using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIExtensions.Demo
{
    public class CopyItemOnStart : MonoBehaviour
    {
        [FormerlySerializedAs("origin")]
        [SerializeField]
        private GameObject m_Origin;

        [FormerlySerializedAs("count")]
        [SerializeField]
        private int m_Count;

        private void Start()
        {
            if (!m_Origin) return;
            m_Origin.SetActive(false);

            var parent = m_Origin.transform.parent;
            for (var i = 0; i < m_Count; i++)
            {
                var go = Instantiate(m_Origin, parent, false);
                go.name = $"{m_Origin.name} {i + 1}";
                go.hideFlags = HideFlags.DontSave;
                go.SetActive(true);
            }
        }
    }
}
