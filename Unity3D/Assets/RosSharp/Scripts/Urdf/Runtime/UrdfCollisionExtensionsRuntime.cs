using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfCollisionExtensionsRuntime {
        public static void Create(Transform parent, GeometryTypes type, Transform visualToCopy = null) {
            GameObject collisionObject = new GameObject("unnamed");
            collisionObject.transform.SetParentAndAlign(parent);

            UrdfCollision urdfCollision = collisionObject.AddComponent<UrdfCollision>();
            urdfCollision.geometryType = type;

            if (visualToCopy != null) {
                if (urdfCollision.geometryType == GeometryTypes.Mesh)
                    UrdfGeometryCollisionRuntime.CreateMatchingMeshCollision(collisionObject.transform, visualToCopy);
                else
                    UrdfGeometryCollisionRuntime.Create(collisionObject.transform, type);

                //copy transform values from corresponding UrdfVisual
                collisionObject.transform.position = visualToCopy.position;
                collisionObject.transform.localScale = visualToCopy.localScale;
                collisionObject.transform.rotation = visualToCopy.rotation;
            } else
                UrdfGeometryCollisionRuntime.Create(collisionObject.transform, type);

        }

        public static void Create(Transform parent, Link.Collision collision, Link.Visual visual = null) {
            GameObject collisionObject = new GameObject("unnamed");
            collisionObject.transform.SetParentAndAlign(parent);
            UrdfCollision urdfCollision = collisionObject.AddComponent<UrdfCollision>();
            urdfCollision.geometryType = UrdfGeometryRuntime.GetGeometryType(collision.geometry);

            UrdfGeometryCollisionRuntime.Create(collisionObject.transform, urdfCollision.geometryType, collision.geometry, visual?.geometry);
            UrdfOrigin.ImportOriginData(collisionObject.transform, collision.origin);
        }
    }
}
