using System;
using System.Linq;
using System.Reflection;
using Coffee.UIExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CFX_Demo_With_UIParticle : MonoBehaviour
{
    private UIParticle UiParticle;
    private Toggle spawnOnUI;
    private MonoBehaviour demo;

    // Start is called before the first frame update
    private void Start()
    {
        UiParticle = GetComponentInChildren<UIParticle>();
        spawnOnUI = GetComponentInChildren<Toggle>();

        demo = FindObjectOfType("CFX_Demo_New") as MonoBehaviour
               ?? FindObjectOfType("WFX_Demo_New") as MonoBehaviour;

        SetCanvasWidth(800);
        SetCanvasRenderOverlay(true);
    }

    private Object FindObjectOfType(string typeName)
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.Name == typeName);

        return type == null ? null : FindObjectOfType(type);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!spawnOnUI.isOn || !demo || !Input.GetMouseButtonDown(0)) return;

        foreach (Transform child in UiParticle.transform)
        {
            Destroy(child.gameObject);
        }

        var particle = demo.GetType()
            .GetMethod("spawnParticle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Invoke(demo, new object[0]) as GameObject;
        particle.transform.localScale = Vector3.one;
        UiParticle.SetParticleSystemInstance(particle);
    }

    public void SetCanvasWidth(int width)
    {
        var scaler = GetComponentInParent<CanvasScaler>();
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0;
        var resolution = scaler.referenceResolution;
        resolution.x = width;
        scaler.referenceResolution = resolution;
    }

    public void SetCanvasRenderOverlay(bool enable)
    {
        var canvas = GetComponentInParent<Canvas>();
        if (enable)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
            canvas.worldCamera = Camera.main;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 5;
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
