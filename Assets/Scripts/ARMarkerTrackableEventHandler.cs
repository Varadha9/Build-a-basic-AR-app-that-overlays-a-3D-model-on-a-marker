using System;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

namespace ARApp.MarkerTracking
{
    /// <summary>
    /// Handles Vuforia tracking events for 2D planar image markers,
    /// triggering smooth model reveal animations, sound effects, and UI state updates.
    /// </summary>
    public class ARMarkerTrackableEventHandler : DefaultObserverEventHandler
    {
        [Header("Model & Effects References")]
        [SerializeField] private GameObject modelRoot;

        [Header("Animation Settings")]
        [SerializeField] private bool usePopInAnimation = true;
        [SerializeField] private float animationDuration = 0.6f;
        [SerializeField] private Vector3 targetScale = Vector3.one;

        [Header("Events")]
        public UnityEvent onTargetAcquired;
        public UnityEvent onTargetDismissed;

        private bool isTracking = false;
        private float animationTimer = 0f;
        private bool isAnimating = false;

        public bool IsTracking => isTracking;

        protected override void Start()
        {
            base.Start();

            if (modelRoot != null)
            {
                targetScale = modelRoot.transform.localScale;
                modelRoot.SetActive(false);
            }
        }

        protected override void OnTrackingFound()
        {
            base.OnTrackingFound();
            isTracking = true;

            if (modelRoot != null)
            {
                modelRoot.SetActive(true);

                if (usePopInAnimation)
                {
                    modelRoot.transform.localScale = Vector3.zero;
                    animationTimer = 0f;
                    isAnimating = true;
                }
                else
                {
                    modelRoot.transform.localScale = targetScale;
                }
            }

            ARStatusUI.Instance?.SetTrackingState(true, mObserverBehaviour != null ? mObserverBehaviour.TargetName : "AR_Marker_Target");
            onTargetAcquired?.Invoke();
            Debug.Log("[AR] Marker Target Acquired successfully!");
        }

        protected override void OnTrackingLost()
        {
            base.OnTrackingLost();
            isTracking = false;
            isAnimating = false;

            if (modelRoot != null)
            {
                modelRoot.SetActive(false);
            }

            ARStatusUI.Instance?.SetTrackingState(false, mObserverBehaviour != null ? mObserverBehaviour.TargetName : "AR_Marker_Target");
            onTargetDismissed?.Invoke();
            Debug.Log("[AR] Marker Target Lost.");
        }

        private void Update()
        {
            if (isAnimating && modelRoot != null)
            {
                animationTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(animationTimer / animationDuration);
                float scaleFactor = EvaluateBounce(progress);
                modelRoot.transform.localScale = targetScale * scaleFactor;

                if (progress >= 1f)
                {
                    modelRoot.transform.localScale = targetScale;
                    isAnimating = false;
                }
            }
        }

        private float EvaluateBounce(float t)
        {
            return 1f + 0.25f * Mathf.Sin(t * Mathf.PI * 3f) * (1f - t);
        }
    }
}
