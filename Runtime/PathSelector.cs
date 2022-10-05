namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using System;
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Allows to select <see cref="RobotPath"/>s from <see cref="PathFileHandler"/>.
    /// </summary>
    /// <remarks>
    /// This component is to bew used in UI logic, so it aims to be as safe as possible.
    /// Even selecting non-existent path index will fall back to the closest available path.
    /// The only case where no path is selected can happen when there are no paths available in the connected <see cref="pathFileHandler"/>.
    /// </remarks>
    public class PathSelector : MonoBehaviour
    {
        #region Data

        public PathFileHandler pathFileHandler;

        [SerializeField]
        [Restricted(RestrictedAttribute.Restrictions.ReadOnlyAlways)]
        private int selectedPathIndex;

        public int SelectedPathIndex
        {
            get
            {
                selectedPathIndex = GetClosestValidPathIndex(selectedPathIndex);
                return selectedPathIndex;
            }
        }

        public RobotPath SelectedPath => (SelectedPathIndex >= 0) ? pathFileHandler.paths[SelectedPathIndex] : null;

        #endregion

        #region Methods

        public void SelectPathByIndex(int i) { selectedPathIndex = GetClosestValidPathIndex(i); }

        private int GetClosestValidPathIndex(int i)
        {
            if (!gameObject.activeInHierarchy)
            {
                // Special case: no path should be selected if this component is inactive
                return -1;
            }
            if (pathFileHandler.paths.Count == 0)
            {
                // Special case: no paths available for selection
                return -1;
            }

            int totalPaths = pathFileHandler.paths.Count;

            if (i < 0) i = 0;
            if (i >= totalPaths) i = totalPaths - 1;

            return i;
        }

        #endregion
    }
}