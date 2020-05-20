using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfVisualExtensionsRuntime {
        public static void Create(Transform parent, GeometryTypes type) {
            GameObject visualObject = new GameObject("unnamed");
            visualObject.transform.SetParentAndAlign(parent);
            UrdfVisual urdfVisual = visualObject.AddComponent<UrdfVisual>();

            urdfVisual.GeometryType = type;
            UrdfGeometryVisualRuntime.Create(visualObject.transform, type);
            UnityEditor.EditorGUIUtility.PingObject(visualObject);
        }

        public static void Create(Transform parent, Link.Visual visual, bool useUrdfMaterials = false) {
            GameObject visualObject = new GameObject(visual.name ?? "unnamed");
            visualObject.transform.SetParentAndAlign(parent);
            UrdfVisual urdfVisual = visualObject.AddComponent<UrdfVisual>();

            urdfVisual.GeometryType = UrdfGeometryRuntime.GetGeometryType(visual.geometry);
            UrdfGeometryVisualRuntime.Create(visualObject.transform, urdfVisual.GeometryType, visual.geometry);

            if (useUrdfMaterials) {
                UrdfMaterialRuntime.SetUrdfMaterial(visualObject, visual.material);
            }

            UrdfOrigin.ImportOriginData(visualObject.transform, visual.origin);
        }

        public static void AddCorrespondingCollision(this UrdfVisual urdfVisual) {
            UrdfCollisions collisions = urdfVisual.GetComponentInParent<UrdfLink>().GetComponentInChildren<UrdfCollisions>();
            UrdfCollisionExtensionsRuntime.Create(collisions.transform, urdfVisual.GeometryType, urdfVisual.transform);
        }
    }
}
