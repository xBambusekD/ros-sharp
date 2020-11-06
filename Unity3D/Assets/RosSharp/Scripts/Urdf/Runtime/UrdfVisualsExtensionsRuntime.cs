using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfVisualsExtensionsRuntime {
        public static void Create(Transform parent, List<Link.Visual> visuals = null, bool useColliderInVisuals = false, bool useUrdfMaterials = false) {
            GameObject visualsObject = new GameObject("Visuals");
            visualsObject.transform.SetParentAndAlign(parent);
            UrdfVisuals urdfVisuals = visualsObject.AddComponent<UrdfVisuals>();

            visualsObject.hideFlags = HideFlags.NotEditable;
            urdfVisuals.hideFlags = HideFlags.None;

            if (visuals != null) {
                foreach (Link.Visual visual in visuals) {
                    UrdfVisualExtensionsRuntime.Create(urdfVisuals.transform, visual, useColliderInVisuals, useUrdfMaterials);
                }
            }
        }
    }
}
