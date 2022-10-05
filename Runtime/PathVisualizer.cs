namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// Visualizes the selected path using markers and a line.
    /// </summary>
    public class PathVisualizer : MonoBehaviour
    {
        #region Data

        /// <summary>
        /// Defines which path will be visualized.
        /// </summary>
        public PathSelector pathSelector;

        /// <summary>
        /// Coordinate system that the path points will be displayed in.
        /// </summary>
        public Transform pathCoordinateSpaceOrigin;

        /// <summary>
        /// Prefab of the object representing a point in the path. 
        /// </summary>
        public GameObject pathPointMarkerPrefab;

        public LineRenderer lineRenderer;

        private readonly List<GameObject> pathPointMarkers = new();

        private Transform markersContainer;

        private RobotPath SelectedPath => pathSelector.SelectedPath;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            markersContainer = new GameObject("Path Markers").transform;
            markersContainer.SetParent(transform, false);
        }

        private void OnDisable() { Destroy(markersContainer); }

        private void LateUpdate() { UpdateVisual(); }

        #endregion

        #region Methods

        /// <summary>
        /// Updates visual elements representing current path.
        /// </summary>
        private void UpdateVisual()
        {
            if (!SelectedPath)
            {
                RemoveAllMarkers();
                UpdateLine();
                return;
            }

            EnsureEnoughMarkers();
            UpdateMarkersPoses();
            UpdateMarkersData();
            UpdateLine();
        }

        private void RemoveAllMarkers()
        {
            foreach (var marker in pathPointMarkers) { Destroy(marker); }

            pathPointMarkers.Clear();
        }

        /// <summary>
        /// Makes sure that the number of currently spawned path point markers matches the number of points in the selected path.
        /// </summary>
        private void EnsureEnoughMarkers()
        {
            int totalPoints = SelectedPath.pointsPose.Count;
            int availableMarkers = pathPointMarkers.Count;

            // Spawn missing markers
            int numberToSpawn = totalPoints - availableMarkers;
            for (int i = 0; i < numberToSpawn; i++)
            {
                var newMarker = Instantiate(pathPointMarkerPrefab, markersContainer);
                pathPointMarkers.Add(newMarker);
            }

            // Destroy redundant markers
            int numberToDestroy = availableMarkers - totalPoints;
            for (int i = 0; i < numberToDestroy; i++)
            {
                var markerToDestroy = pathPointMarkers.Last();
                pathPointMarkers.Remove(markerToDestroy);
                Destroy(markerToDestroy);
            }
        }

        /// <summary>
        /// Makes sure that the poses path point markers matches the points in the selected path.
        /// </summary>
        private void UpdateMarkersPoses()
        {
            int totalPoints = SelectedPath.pointsPose.Count;
            for (int i = 0; i < totalPoints; i++)
            {
                var point = SelectedPath.pointsPose[i];
                var marker = pathPointMarkers[i];

                var markerPoseWorld = new Pose
                {
                    position = pathCoordinateSpaceOrigin.TransformPoint(point.position),
                    rotation = pathCoordinateSpaceOrigin.rotation * point.rotation
                };

                marker.transform.SetPositionAndRotation(markerPoseWorld.position, markerPoseWorld.rotation);
            }
        }

        /// <summary>
        /// Updates info (like point names) on the point markers.
        /// </summary>
        private void UpdateMarkersData()
        {
            for (int i = 0; i < pathPointMarkers.Count; i++)
            {
                var marker = pathPointMarkers[i];
                var markerLabel = marker.GetComponentInChildren<TMP_Text>();

                string label = $"P{i}";
                markerLabel.text = label;
            }
        }

        /// <summary>
        /// Updates path line renderer to match the visualized path.
        /// </summary>
        private void UpdateLine()
        {
            if (!lineRenderer)
            {
                Debug.LogError($"{nameof(LineRenderer)} component is required to visualize the path. Please add it manually.", this);
                return;
            }

            // Only show line if any path is selected
            lineRenderer.enabled = (SelectedPath != null);
            
            // If no path is selected, nothing else to do here
            if (!SelectedPath) return;

            // Populate line from path data
            int totalPoints = SelectedPath.pointsPose.Count;
            var pointPositions = new Vector3[totalPoints];
            for (int i = 0; i < totalPoints; i++)
            {
                var pointLocalSpace = SelectedPath.pointsPose[i];
                
                // Convert path points to world space
                var pointPositionWorldSpace = pathCoordinateSpaceOrigin.TransformPoint(pointLocalSpace.position);
                pointPositions[i] = pointPositionWorldSpace;
            }
            
            lineRenderer.positionCount = SelectedPath.pointsPose.Count;
            lineRenderer.SetPositions(pointPositions);
        }

        #endregion
    }
}