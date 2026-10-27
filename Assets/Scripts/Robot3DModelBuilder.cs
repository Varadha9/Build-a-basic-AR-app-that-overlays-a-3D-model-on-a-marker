using UnityEngine;

namespace ARApp.MarkerTracking
{
    /// <summary>
    /// Builds a multi-component 3D Sci-Fi Drone / Robot model at runtime
    /// with rotating thruster rings, glowing energy core, and metallic materials.
    /// </summary>
    public class Robot3DModelBuilder : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material bodyMaterial;
        [SerializeField] private Material glowMaterial;
        [SerializeField] private Material goldMaterial;

        private void Start()
        {
            ConstructModel();
        }

        private void ConstructModel()
        {
            // Create default materials if not set
            if (bodyMaterial == null)
            {
                bodyMaterial = new Material(Shader.Find("Standard"));
                bodyMaterial.color = new Color(0.18f, 0.22f, 0.28f);
                bodyMaterial.SetFloat("_Metallic", 0.85f);
                bodyMaterial.SetFloat("_Glossiness", 0.75f);
            }

            if (glowMaterial == null)
            {
                glowMaterial = new Material(Shader.Find("Standard"));
                glowMaterial.color = new Color(0.0f, 0.85f, 1.0f);
                glowMaterial.EnableKeyword("_EMISSION");
                glowMaterial.SetColor("_EmissionColor", new Color(0.0f, 0.85f, 1.0f) * 2.5f);
            }

            if (goldMaterial == null)
            {
                goldMaterial = new Material(Shader.Find("Standard"));
                goldMaterial.color = new Color(1.0f, 0.75f, 0.1f);
                goldMaterial.SetFloat("_Metallic", 0.9f);
                goldMaterial.SetFloat("_Glossiness", 0.85f);
            }

            // 1. Central Core Fuselage
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "CentralCore";
            core.transform.SetParent(transform, false);
            core.transform.localPosition = new Vector3(0, 0.12f, 0);
            core.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            core.GetComponent<Renderer>().material = bodyMaterial;

            // 2. Glowing Sensor Eye
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "SensorEye";
            eye.transform.SetParent(core.transform, false);
            eye.transform.localPosition = new Vector3(0, 0, 0.45f);
            eye.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            eye.GetComponent<Renderer>().material = glowMaterial;

            // 3. Core Point Light
            GameObject lightObj = new GameObject("CoreLight");
            lightObj.transform.SetParent(core.transform, false);
            Light coreLight = lightObj.AddComponent<Light>();
            coreLight.type = LightType.Point;
            coreLight.color = new Color(0.1f, 0.9f, 1.0f);
            coreLight.range = 0.5f;
            coreLight.intensity = 2.0f;

            // 4. Outer Rotating Gyro Ring (Yaw)
            GameObject gyroRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gyroRing.name = "GyroRing_Horizontal";
            gyroRing.transform.SetParent(transform, false);
            gyroRing.transform.localPosition = new Vector3(0, 0.12f, 0);
            gyroRing.transform.localScale = new Vector3(0.24f, 0.015f, 0.24f);
            gyroRing.GetComponent<Renderer>().material = goldMaterial;
            gyroRing.AddComponent<ContinuousRotator>().rotationAxis = new Vector3(0, 45f, 0);

            // 5. 4 Quad Rotor Arms
            Vector3[] armOffsets = new Vector3[]
            {
                new Vector3(0.12f, 0.12f, 0.12f),
                new Vector3(-0.12f, 0.12f, 0.12f),
                new Vector3(0.12f, 0.12f, -0.12f),
                new Vector3(-0.12f, 0.12f, -0.12f)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arm.name = $"RotorArm_{i}";
                arm.transform.SetParent(transform, false);
                arm.transform.localPosition = armOffsets[i];
                arm.transform.localScale = new Vector3(0.06f, 0.01f, 0.06f);
                arm.GetComponent<Renderer>().material = bodyMaterial;

                // Propeller blades on each rotor
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blade.name = "Blade";
                blade.transform.SetParent(arm.transform, false);
                blade.transform.localPosition = new Vector3(0, 1.2f, 0);
                blade.transform.localScale = new Vector3(1.4f, 0.1f, 0.2f);
                blade.GetComponent<Renderer>().material = glowMaterial;
                blade.AddComponent<ContinuousRotator>().rotationAxis = new Vector3(0, (i % 2 == 0 ? 360f : -360f), 0);
            }

            // 6. Base Pedestal / Landing Matrix
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "PedestalBase";
            pedestal.transform.SetParent(transform, false);
            pedestal.transform.localPosition = new Vector3(0, 0.02f, 0);
            pedestal.transform.localScale = new Vector3(0.20f, 0.015f, 0.20f);
            pedestal.GetComponent<Renderer>().material = bodyMaterial;

            // 7. Add Touch & Gesture Interaction Controller
            gameObject.AddComponent<ModelInteractionController>();
        }
    }

    public class ContinuousRotator : MonoBehaviour
    {
        public Vector3 rotationAxis = new Vector3(0, 30f, 0);

        private void Update()
        {
            transform.Rotate(rotationAxis * Time.deltaTime, Space.Self);
        }
    }
}
