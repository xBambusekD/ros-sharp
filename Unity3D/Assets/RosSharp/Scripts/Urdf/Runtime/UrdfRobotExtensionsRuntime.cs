using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfRobotExtensionsRuntime {

        /// <summary>
        /// Imports robot from specified urdf given in filename.
        /// </summary>
        /// <param name="filename">Full path urdf filename.</param>
        /// <param name="useColliderInVisuals">If set to true, import will create the mesh collider in Visuals with the copy of mesh from <visual> urdf tag.</param>
        /// <param name="useUrdfMaterials">If set to true, import will use the materials specified in urdf.</param>
        /// <returns></returns>
        public static UrdfRobot Create(string filename, bool useColliderInVisuals = false, bool useUrdfMaterials = false) {

            UrdfAssetImporterRuntime.Instance.CurrentUrdfRoot = Path.GetDirectoryName(filename);

            RosSharp.Urdf.Robot robot = new RosSharp.Urdf.Robot(filename);

            GameObject robotGameObject = new GameObject(robot.name);
            UrdfRobot urdfRobot = robotGameObject.AddComponent<UrdfRobot>();

            //UrdfAssetPathHandler.SetPackageRoot(Path.GetDirectoryName(robot.filename));
            //UrdfMaterialRuntime.InitializeRobotMaterials(robot);
            UrdfPlugins.Create(robotGameObject.transform, robot.plugins);

            UrdfLinkExtensionsRuntime.Create(robotGameObject.transform, robot.root, useColliderInVisuals: useColliderInVisuals, useUrdfMaterials: useUrdfMaterials);

            return urdfRobot;
        }
    }
}
