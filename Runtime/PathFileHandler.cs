namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Manages creation and loading of files containing programmed robot Paths.
    /// </summary>
    public class PathFileHandler : MonoBehaviour
    {
        #region Data

        /// <summary>
        /// List of paths loaded by the application.
        /// </summary>
        public List<RobotPath> paths = new();

        #endregion

    #if UNITY_EDITOR
        #region Editor Callbacks
        
        [SerializeField]
        [Restricted(RestrictedAttribute.Restrictions.ReadOnlyAlways)]
        private string defaultFileLocation;

        private void OnValidate() { defaultFileLocation = GetDefaultJsonSaveLocation(); }

        #endregion
    #endif
        
        #region Unity Callbacks

        private void OnEnable() { ReloadPathsFromDefaultLocation(); }

        #endregion

        #region Internal Methods (Paths)

        /// <summary>
        /// Retrieves the default application persistent data directory for path files.
        /// </summary>
        private static string GetDefaultJsonSaveLocation() { return Path.Join(Application.persistentDataPath, "Saved Robot Trajectories"); }

        /// <summary>
        /// Retrieves a full path to the save file location for the given <see cref="RobotPath"/>. 
        /// </summary>
        /// <param name="path"><see cref="RobotPath"/> to get the save location for.</param>
        private static string GetFileSaveLocationFromPath(RobotPath path)
        {
            string saveLocation = GetDefaultJsonSaveLocation();
            return Path.Join(saveLocation, $"{path.pathName}.json");
        }

        #endregion

        #region Internal Methods (Saving and Loading)

        private void ReloadPathsFromDefaultLocation()
        {
            paths = LoadPathsFromDefaultLocation();
        }
        
        /// <summary>
        /// Loads robot paths from JSON files in the default disk location.
        /// </summary>
        private List<RobotPath> LoadPathsFromDefaultLocation()
        {
            // Creates a list to hold loaded RobotPaths 
            var listOfPathsScriptableObjects = new List<RobotPath>();

            // Ensure the save directory exists
            string saveLocation = GetDefaultJsonSaveLocation();
            if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);

            var dir = new DirectoryInfo(saveLocation);
            foreach (var file in dir.GetFiles("*.json"))
            {
                var loadedPath = LoadPathFromJsonFile(file);
                listOfPathsScriptableObjects.Add(loadedPath);
            }

            return listOfPathsScriptableObjects;
        }

        /// <summary>
        /// Updates the ScriptableObject with the data saved in the JSON file.
        /// </summary>
        /// <param name="fileInfo">Path to the JSON file to load the path from.</param>
        /// <returns>Loaded <see cref="RobotPath"/> instance.</returns>
        private RobotPath LoadPathFromJsonFile(FileInfo fileInfo)
        {
            // Creating a RobotPath scriptable object instance
            var newRobotPath = ScriptableObject.CreateInstance<RobotPath>();

            // Writing the JSON file info in the ScriptableObject
            string json = File.ReadAllText(fileInfo.FullName);
            JsonUtility.FromJsonOverwrite(json, newRobotPath);

            return newRobotPath;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new path file in the default location and returns it's Scriptable Object.
        /// </summary>
        /// <returns>Created path object.</returns>
        public RobotPath CreateNewEmptyPath()
        {
            // Creating a RobotPath scriptable object instance
            var newRobotPath = ScriptableObject.CreateInstance<RobotPath>();

            int totalPaths = paths.Count;
            newRobotPath.pathName = $"Path {totalPaths + 1}";
            
            SavePathToDefaultLocation(newRobotPath);
            
            ReloadPathsFromDefaultLocation();

            return newRobotPath;
        }

        /// <summary>
        /// Saves the give <see cref="RobotPath"/> to a JSON file in the default location.
        /// </summary>
        /// <param name="path"><see cref="RobotPath"/> to save.</param>
        public void SavePathToDefaultLocation(RobotPath path)
        {
            string json = JsonUtility.ToJson(path);
            string filePath = GetFileSaveLocationFromPath(path);

            File.WriteAllText(filePath, json);
            
            Debug.Log($"Saved path '{path.pathName}' to '{filePath}'.");
        }

        /// <summary>
        /// Deletes the saved JSON file corresponding to the given <see cref="RobotPath"/>, if it exists.
        /// </summary>
        /// <param name="path"><see cref="RobotPath"/> to delete.</param>
        public void DeletePathFile(RobotPath path)
        {
            string filePath = GetFileSaveLocationFromPath(path);

            if (File.Exists(filePath)) 
            {
                File.Delete(filePath);
                ReloadPathsFromDefaultLocation();
                
                Debug.Log($"Deleted path file '{filePath}'.");
            }
            else
            {
                Debug.LogWarning($"Did not delete the path file '{filePath}', as it does not exist.");
            }
        }

        #endregion
    }
}