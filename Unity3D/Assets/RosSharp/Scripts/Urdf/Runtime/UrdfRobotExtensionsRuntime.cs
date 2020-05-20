using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfRobotExtensionsRuntime {

        public static UrdfRobot Create(string filename, bool useUrdfMaterials = false) {

            UrdfAssetImporterRuntime.Instance.CurrentUrdfRoot = Path.GetDirectoryName(filename);

            RosSharp.Urdf.Robot robot = new RosSharp.Urdf.Robot(filename);

            GameObject robotGameObject = new GameObject(robot.name);
            UrdfRobot urdfRobot = robotGameObject.AddComponent<UrdfRobot>();

            //UrdfAssetPathHandler.SetPackageRoot(Path.GetDirectoryName(robot.filename));
            //UrdfMaterialRuntime.InitializeRobotMaterials(robot);
            UrdfPlugins.Create(robotGameObject.transform, robot.plugins);

            UrdfLinkExtensionsRuntime.Create(robotGameObject.transform, robot.root, useUrdfMaterials: useUrdfMaterials);

            return urdfRobot;
        }
    }
}
