using UnityEngine;

namespace ARApp.MarkerTracking
{
    /// <summary>
    /// Enables interactive touch manipulation (single-finger 360 rotation,
    /// two-finger pinch-to-scale) and idle rotation for the overlaid 3D model.
    /// </summary>
    public class ModelInteractionController : MonoBehaviour
    {
        [Header("Idle Rotation")]
        [SerializeField] private bool enableIdleRotation = true;
        [SerializeField] private float idleRotationSpeed = 25f;

        [Header("Touch Gestures")]
        [SerializeField] private float rotationSensitivity = 0.4f;
        [SerializeField] private float pinchSensitivity = 0.005f;
        [SerializeField] private float minScale = 0.2f;
        [SerializeField] private float maxScale = 3.0f;

        private Vector3 initialScale;
        private Quaternion initialRotation;
        private bool isInteracting = false;
        private float lastInteractionTime = 0f;

        private void Awake()
        {
            initialScale = transform.localScale;
            initialRotation = transform.localRotation;
        }

        private void Update()
        {
            HandleTouchInput();

            // Perform smooth idle rotation when user is not actively manipulating model
            if (enableIdleRotation && !isInteracting && (Time.time - lastInteractionTime > 1.5f))
            {
                transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    isInteracting = true;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    isInteracting = true;
                    lastInteractionTime = Time.time;

                    float deltaX = touch.deltaPosition.x * rotationSensitivity;
                    float deltaY = touch.deltaPosition.y * rotationSensitivity;

                    transform.Rotate(Vector3.up, -deltaX, Space.World);
                    transform.Rotate(Vector3.right, deltaY, Space.World);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isInteracting = false;
                    lastInteractionTime = Time.time;
                }
            }
            else if (Input.touchCount == 2)
            {
                isInteracting = true;
                lastInteractionTime = Time.time;

                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
                Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

                float prevDistance = (prevPos0 - prevPos1).magnitude;
                float currentDistance = (touch0.position - touch1.position).magnitude;
                float deltaDistance = currentDistance - prevDistance;

                float scaleChange = deltaDistance * pinchSensitivity;
                Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

                float clampedX = Mathf.Clamp(newScale.x, initialScale.x * minScale, initialScale.x * maxScale);
                transform.localScale = new Vector3(clampedX, clampedX, clampedX);
            }
            else
            {
                if (isInteracting)
                {
                    isInteracting = false;
                    lastInteractionTime = Time.time;
                }
            }

            // Double tap to reset
            if (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 2)
            {
                ResetTransform();
            }
        }

        public void ResetTransform()
        {
            transform.localRotation = initialRotation;
            transform.localScale = initialScale;
        }
    }
}
