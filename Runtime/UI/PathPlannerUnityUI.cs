namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;

    public class PathPlannerUnityUI : MonoBehaviour
    {
        #region Data

        public PathSelector pathSelector;
        public PathExecutor pathExecutor;
        public TMP_Dropdown pathsDropdown;
        
        public PathFileHandler PathFileHandler => pathSelector.pathFileHandler;

        #endregion

        #region Unity Callbacks

        private void OnEnable() { pathsDropdown.onValueChanged.AddListener(SelectPathFromDropdown); }

        private void OnDisable() { pathsDropdown.onValueChanged.RemoveListener(SelectPathFromDropdown); }

        private void Update() { RefreshDropdown(); }

        #endregion

        #region Main Methods

        private void RefreshDropdown()
        {
            var paths = PathFileHandler.paths;

            string allPathNames = string.Join(", ", paths.Select((p) => p.pathName));
            // Debug.Log($"refresh dropdown with: {allPathNames}");

            pathsDropdown.ClearOptions();
            var options = new List<string>();

            if (paths.Count >= 1)
            {
                foreach (var p in paths) { options.Add(p.pathName); }
            }
            else { options = new List<string>() { "No paths" }; }

            // Populate the dropdown
            pathsDropdown.AddOptions(options);
            pathsDropdown.RefreshShownValue();

            // Preserve selection
            pathsDropdown.SetValueWithoutNotify(pathSelector.SelectedPathIndex);
        }

        public void QuitApplication() { Application.Quit(); }

        public void CreateNewPath()
        {
            PathFileHandler.CreateNewEmptyPath();
            pathSelector.SelectPathByIndex(int.MaxValue);
        }

        public void SelectPathFromDropdown(int i)
        {
            pathSelector.SelectPathByIndex(i);
        }

        #endregion

        #region Methods (Individual Path)

        public void DeleteSelectedPath()
        {
            if (pathSelector.SelectedPath == null)
            {
                Debug.LogWarning("Cannot delete selected path, because no path is selected.");
                return;
            }

            pathSelector.pathFileHandler.DeletePathFile(pathSelector.SelectedPath);
        }

        public void ExecuteSelectedPath() { pathExecutor.ExecuteSelectedPathNoWait(); }

        #endregion
    }
}