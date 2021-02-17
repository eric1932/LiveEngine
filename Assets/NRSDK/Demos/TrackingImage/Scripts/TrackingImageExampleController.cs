﻿namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Controller for TrackingImage example.
    /// </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/image-tracking")]
    public class TrackingImageExampleController : MonoBehaviour
    {
        // A prefab for visualizing an TrackingImage.
        public TrackingImageVisualizer TrackingImageVisualizerPrefab;

        // The overlay containing the fit to scan user guide.
        //public GameObject FitToScanOverlay;

        private Dictionary<int, TrackingImageVisualizer> m_Visualizers
            = new Dictionary<int, TrackingImageVisualizer>();

        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        private bool virtualImageTrackingEnabled = false;

        public void Update()
        {
#if UNITY_EDITOR
            if (virtualImageTrackingEnabled)
            {
                return;
            }
            else
            {
                TrackingImageVisualizer visualizer = null;
                NRDebugger.Log("Create new TrackingImageVisualizer!");
                visualizer = (TrackingImageVisualizer) Instantiate(
                    TrackingImageVisualizerPrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity);
                // visualizer.transform.parent = transform;
                visualizer.transform.parent = null;  // add to root of scene
                visualizer.transform.localPosition = new Vector3(0, 0, 5f);

                virtualImageTrackingEnabled = true;  // set flag

                Destroy(gameObject);  // STOP image tracking

                return;
            }
#endif

#if !UNITY_EDITOR
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return;
            }
#endif
            // Get updated augmented images for this frame.
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempTrackingImages)
            {
                TrackingImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                if (image.GetTrackingState() == TrackingState.Tracking && visualizer == null)
                {
                    NRDebugger.Log("Create new TrackingImageVisualizer!");
                    // Create an anchor to ensure that NRSDK keeps tracking this augmented image.
                    visualizer = (TrackingImageVisualizer)Instantiate(TrackingImageVisualizerPrefab, image.GetCenterPose().position, image.GetCenterPose().rotation);
                    visualizer.Image = image;
                    // visualizer.transform.parent = transform;
                    visualizer.transform.parent = null;  // add to root of scene
                    m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);
                    Destroy(gameObject);  // STOP image tracking
                }
                else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.GetDataBaseIndex());
                    Destroy(visualizer.gameObject);
                }

                //FitToScanOverlay.SetActive(false);
            }

        }

        public void EnableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.ENABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }

        public void DisableImageTracking()
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }
    }
}
