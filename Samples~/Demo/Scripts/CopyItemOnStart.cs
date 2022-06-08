using UnityEngine;

namespace Coffee.UIExtensions.Demo
{
    public class CopyItemOnStart : MonoBehaviour
    {
        public GameObject origin;
        public int count;

        private void Start()
        {
            if (!origin) return;
            origin.SetActive(false);

            var parent = origin.transform.parent;
            for (var i = 0; i < count; i++)
            {
                var go = Instantiate(origin, parent, false);
                go.name = string.Format("{0} {1}", origin.name, i + 1);
                go.hideFlags = HideFlags.DontSave;
                go.SetActive(true);
            }
        }
    }
}