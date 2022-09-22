namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Allows to register path points in the given coordinate system using a provided Transform.
    /// </summary>
    public class PathTracingTool : MonoBehaviour
    {
        #region Data

        /// <summary>
        /// Defines to which path the point must be saved.
        /// </summary>
        public PathSelector pathSelector;

        /// <summary>
        /// Transform used to get the poses of new points of the path.
        /// </summary>
        public Transform tool;

        /// <summary>
        /// Coordinate system that the poses will be saved in.
        /// </summary>
        public Transform pathCoordinateSpaceOrigin;

        private RobotPath SelectedPath => pathSelector.SelectedPath;
        
        public PathFileHandler PathFileHandler => pathSelector.pathFileHandler;

        #endregion

        #region Methods

        public void AddPointToSelectedPath()
        {
            // Sanity checks
            if (!pathCoordinateSpaceOrigin) { Debug.LogException(new ApplicationException($"Cannot save path point: '{nameof(pathCoordinateSpaceOrigin)}' Transform is not set.")); }
            if (!SelectedPath) { Debug.LogException(new ApplicationException($"Cannot save path point: no pat is currently selected.\n Select path in the connected {nameof(PathSelector)}.")); }

            // Find tool pose in the path origin coordinates
            var pathPointPose = new Pose
            {
                position = pathCoordinateSpaceOrigin.InverseTransformPoint(tool.position),
                rotation = Quaternion.Inverse(pathCoordinateSpaceOrigin.rotation) * tool.rotation
            };

            // Save to path
            SelectedPath.pointsPose.Add(pathPointPose);
            PathFileHandler.SavePathToDefaultLocation(SelectedPath);
        }

        #endregion
    }
}