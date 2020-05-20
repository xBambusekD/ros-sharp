using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfCollisionsExtensionsRuntime {
        public static void Create(Transform parent, List<Link.Collision> collisions = null, List<Link.Visual> visuals = null) {
            GameObject collisionsObject = new GameObject("Collisions");
            collisionsObject.transform.SetParentAndAlign(parent);
            UrdfCollisions urdfCollisions = collisionsObject.AddComponent<UrdfCollisions>();

            collisionsObject.hideFlags = HideFlags.NotEditable;
            urdfCollisions.hideFlags = HideFlags.None;

            if (collisions != null) {
                foreach (Link.Collision collision in collisions) {
                    Link.Visual visual = null;
                    if (visuals != null) {
                        visual = GetCorrespondingVisual(collision, visuals);
                    }
                    UrdfCollisionExtensionsRuntime.Create(urdfCollisions.transform, collision, visual);
                }
            }
        }

        private static Link.Visual GetCorrespondingVisual(Link.Collision collision, List<Link.Visual> visuals) {
            if (collision.geometry.mesh != null) {
                string collisionFilename = Path.GetFileNameWithoutExtension(collision.geometry.mesh.filename);
                foreach (Link.Visual visual in visuals) {
                    if (visual.geometry.mesh != null) {
                        if (visual.geometry.mesh.filename.Contains(collisionFilename)) {
                            return visual;
                        }
                    }
                }
            }

            return null;
        }
    }
}
