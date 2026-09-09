# Unity ROS Robot Programming

Collection of scripts to record and play back trajectories on connected ROS Robots.

## Installation

Add the package and its TalTech dependencies directly to the Unity project's
`Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=/src/UniTask/Assets/Plugins/UniTask#2.3.1",
    "ee.taltech.ivar.robotics.moveit": "https://github.com/TalTech-IVAR-Lab/Unity-MoveIt-Integration.git#v1.3.0",
    "ee.taltech.ivar.robotics.ros-industrial": "https://github.com/TalTech-IVAR-Lab/Unity-ROS-Industrial-Integration.git#v1.3.1",
    "ee.taltech.ivar.robotics.ros-robot-programming": "https://github.com/TalTech-IVAR-Lab/Unity-ROS-Robot-Programming.git#v1.1.7",
    "io.extendreality.zinnia.unity": "https://github.com/ExtendRealityLtd/Zinnia.Unity.git#v2.0.0"
  }
}
```

Unity does not resolve version-only transitive dependencies from Git. Keep all
five entries in the project manifest unless another package source provides them.
