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
        }

        public static void Create(Transform parent, Link.Visual visual, bool useColliderInVisuals = false, bool useUrdfMaterials = false) {
            GameObject visualObject = new GameObject(visual.name ?? "unnamed");
            visualObject.transform.SetParentAndAlign(parent);
            UrdfVisual urdfVisual = visualObject.AddComponent<UrdfVisual>();

            urdfVisual.GeometryType = UrdfGeometryRuntime.GetGeometryType(visual.geometry);
            UrdfGeometryVisualRuntime.Create(visualObject.transform, urdfVisual.GeometryType, visual.geometry, useColliderInVisuals);

            if (useUrdfMaterials) {
                UrdfMaterialRuntime.SetUrdfMaterial(visualObject, visual.material);
            }

            UrdfOrigin.ImportOriginData(visualObject.transform, visual.origin);
        }
    }
}
