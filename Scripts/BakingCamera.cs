using UnityEngine;

namespace Coffee.UIExtensions
{
    internal class BakingCamera : MonoBehaviour
    {
        static BakingCamera s_Instance;
        private static readonly Vector3 s_OrthoPosition = new Vector3(0, 0, -1000);
        private static readonly Quaternion s_OrthoRotation = Quaternion.identity;

#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
        static BakingCamera s_InstanceForPrefab;

        private static BakingCamera InstanceForPrefab
        {
            get
            {
                // If current scene is prefab mode, create OverlayCamera for editor.
                var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null || !prefabStage.scene.isLoaded) return null;
                if (s_InstanceForPrefab) return s_InstanceForPrefab;

                s_InstanceForPrefab = Create();
                s_InstanceForPrefab.name += " (For Prefab Stage)";
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(s_InstanceForPrefab.gameObject, prefabStage.scene);

                return s_InstanceForPrefab;
            }
        }
#endif

        private static BakingCamera Instance
        {
            get
            {
#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
                var inst = InstanceForPrefab;
                if (inst) return inst;
#endif
                // Find instance in scene, or create new one.
                return s_Instance
                    ? s_Instance
                    : (s_Instance = FindObjectOfType<BakingCamera>() ?? Create());
            }
        }

        private Camera _camera;
        // private int _refCount;

        private static BakingCamera Create()
        {
            var gameObject = new GameObject(typeof(BakingCamera).Name);

            // This camera object is just for internal use
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            var inst = gameObject.AddComponent<BakingCamera>();
            inst._camera = gameObject.AddComponent<Camera>();
            inst._camera.orthographic = true;

            // Turn camera off because particle mesh baker will use only camera matrix
            gameObject.SetActive(false);

            return inst;
        }

        private void Awake()
        {
            if (this == s_Instance)
                DontDestroyOnLoad(gameObject);
        }

        public static Camera GetCamera(Canvas canvas)
        {
            if (!canvas) return Camera.main;

            canvas = canvas.rootCanvas;
            // Adjust camera orthographic size to canvas size
            // for canvas-based coordinates of particles' size and speed.
            var size = ((RectTransform) canvas.transform).rect.size;
            Instance._camera.orthographicSize = Mathf.Max(size.x, size.y) * canvas.scaleFactor;

            var camera = canvas.worldCamera;
            var transform = Instance.transform;
            var rotation = canvas.renderMode != RenderMode.ScreenSpaceOverlay && camera
                ? camera.transform.rotation
                : s_OrthoRotation;

            transform.SetPositionAndRotation(canvas.transform.position + s_OrthoPosition, rotation);
            Instance._camera.orthographic = true;

            return Instance._camera;
        }
    }
}
