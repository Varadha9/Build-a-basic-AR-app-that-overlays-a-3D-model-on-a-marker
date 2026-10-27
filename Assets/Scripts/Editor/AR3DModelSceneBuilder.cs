using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Vuforia;
using ARApp.MarkerTracking;

public static class AR3DModelSceneBuilder
{
    [MenuItem("Vuforia/Build AR 3D Model Marker Scene")]
    public static void BuildScene()
    {
        Debug.Log("[AR3DModelSceneBuilder] Building AR Marker Tracking Scene...");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Directional Light
        var lightGo = new GameObject("Directional Light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1.0f, 0.98f, 0.92f);
        light.intensity = 1.3f;
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Fill Light
        var fillLightGo = new GameObject("Fill Light");
        var fillLight = fillLightGo.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.color = new Color(0.6f, 0.8f, 1.0f);
        fillLight.intensity = 0.5f;
        fillLightGo.transform.rotation = Quaternion.Euler(-30f, 140f, 0f);

        // 2. AR Camera
        var cameraGo = new GameObject("ARCamera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.1f, 0.14f);
        camera.nearClipPlane = 0.05f;
        camera.farClipPlane = 2000f;
        cameraGo.tag = "MainCamera";
        cameraGo.AddComponent<AudioListener>();
        cameraGo.AddComponent<VuforiaBehaviour>();
        cameraGo.transform.position = new Vector3(0, 0.25f, -0.4f);
        cameraGo.transform.rotation = Quaternion.Euler(30f, 0, 0);

        // 3. Image Target GameObject (Planar 2D Marker)
        var targetGo = new GameObject("ImageTarget_ARMarker");
        targetGo.transform.position = Vector3.zero;
        targetGo.transform.rotation = Quaternion.identity;

        var imgTarget = targetGo.AddComponent<ImageTargetBehaviour>();

        // 4. Visual Marker Plane for Reference in Editor
        var markerRef = GameObject.CreatePrimitive(PrimitiveType.Quad);
        markerRef.name = "PhysicalMarkerReference";
        markerRef.transform.SetParent(targetGo.transform, false);
        markerRef.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        markerRef.transform.localScale = new Vector3(0.2f, 0.2f, 1f); // 20cm x 20cm

        var markerTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/AR_Marker_Target.png");
        if (markerTex != null)
        {
            var mat = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"));
            mat.mainTexture = markerTex;
            mat.name = "ARMarker_Mat";
            AssetDatabase.CreateAsset(mat, "Assets/Materials/ARMarker_Mat.mat");
            markerRef.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // 5. Overlaid 3D Model Container
        var modelRoot = new GameObject("AR_3DModel_Overlay");
        modelRoot.transform.SetParent(targetGo.transform, false);
        modelRoot.transform.localPosition = new Vector3(0, 0.01f, 0);
        modelRoot.transform.localScale = Vector3.one * 0.8f;

        // Add 3D model generator & touch controller
        modelRoot.AddComponent<Robot3DModelBuilder>();

        // 6. Wire Trackable Event Handler
        var trackingHandler = targetGo.AddComponent<ARMarkerTrackableEventHandler>();
        
        // Use reflection / serialized object to set private modelRoot and mObserverBehaviour
        var so = new SerializedObject(trackingHandler);
        var propModel = so.FindProperty("modelRoot");
        if (propModel != null)
        {
            propModel.objectReferenceValue = modelRoot;
            so.ApplyModifiedProperties();
        }

        // 7. AR HUD Status UI
        var hudGo = new GameObject("AR_Status_HUD");
        hudGo.AddComponent<ARStatusUI>();

        // Save scene
        string scenePath = "Assets/Scenes/ARMarkerTrackingScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[AR3DModelSceneBuilder] Successfully saved scene: {scenePath}");

        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(scenePath, true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[AR3DModelSceneBuilder] Scene set in EditorBuildSettings.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
