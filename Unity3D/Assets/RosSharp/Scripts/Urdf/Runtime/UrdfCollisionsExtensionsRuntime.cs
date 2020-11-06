using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfCollisionsExtensionsRuntime {
        public static void Create(Transform parent, List<Link.Collision> collisions = null) {
            GameObject collisionsObject = new GameObject("Collisions");
            collisionsObject.transform.SetParentAndAlign(parent);
            UrdfCollisions urdfCollisions = collisionsObject.AddComponent<UrdfCollisions>();

            collisionsObject.hideFlags = HideFlags.NotEditable;
            urdfCollisions.hideFlags = HideFlags.None;

            if (collisions != null) {
                foreach (Link.Collision collision in collisions) {
                    UrdfCollisionExtensionsRuntime.Create(urdfCollisions.transform, collision);
                }
            }
        }
    }
}
