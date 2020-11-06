using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfCollisionExtensionsRuntime {
        public static void Create(Transform parent, GeometryTypes type) {
            GameObject collisionObject = new GameObject("unnamed");
            collisionObject.transform.SetParentAndAlign(parent);

            UrdfCollision urdfCollision = collisionObject.AddComponent<UrdfCollision>();
            urdfCollision.geometryType = type;

            UrdfGeometryCollisionRuntime.Create(collisionObject.transform, type);
        }

        public static void Create(Transform parent, Link.Collision collision) {
            GameObject collisionObject = new GameObject("unnamed");
            collisionObject.transform.SetParentAndAlign(parent);
            UrdfCollision urdfCollision = collisionObject.AddComponent<UrdfCollision>();
            urdfCollision.geometryType = UrdfGeometryRuntime.GetGeometryType(collision.geometry);

            UrdfGeometryCollisionRuntime.Create(collisionObject.transform, urdfCollision.geometryType, collision.geometry);
            UrdfOrigin.ImportOriginData(collisionObject.transform, collision.origin);
        }
    }
}
