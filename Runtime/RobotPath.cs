using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class RobotPath : ScriptableObject
{
    [FormerlySerializedAs("pathname")]
    public string pathName;
    public List<Pose> pointsPose;
    public int pathNumber;
    public float pathWidth;     
    
    //Not necessary anymore in build version
    /*public void SetDirty()
    {
        EditorUtility.SetDirty(this);
    }
    public void SaveDirty()
    {
        AssetDatabase.SaveAssetIfDirty(this);
    }*/
}
