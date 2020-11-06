using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public class UrdfGeometryCollisionRuntime : UrdfGeometryRuntime {
        public static void Create(Transform parent, GeometryTypes geometryType, Link.Geometry geometry = null) {
            GameObject geometryGameObject = null;

            switch (geometryType) {
                case GeometryTypes.Box:
                    geometryGameObject = new GameObject(geometryType.ToString());
                    geometryGameObject.AddComponent<BoxCollider>();
                    break;
                case GeometryTypes.Cylinder:
                    geometryGameObject = CreateCylinderCollider();
                    break;
                case GeometryTypes.Sphere:
                    geometryGameObject = new GameObject(geometryType.ToString());
                    geometryGameObject.AddComponent<SphereCollider>();
                    break;
                case GeometryTypes.Mesh:
                    if (geometry != null)
                        geometryGameObject = CreateMeshCollider(geometry.mesh);
                    else {
                        geometryGameObject = new GameObject(geometryType.ToString());
                        geometryGameObject.AddComponent<MeshCollider>();
                    }
                    break;
            }

            if (geometryGameObject != null) {
                geometryGameObject.transform.SetParentAndAlign(parent);
                if (geometry != null)
                    SetScale(parent, geometry, geometryType);
            }
        }

        private static GameObject CreateMeshCollider(Link.Geometry.Mesh mesh) {
            GameObject meshObject = UrdfAssetImporterRuntime.Instance.ImportUrdfAsset(mesh.filename, useCollidersOnly:true);
            return meshObject;
        }

        private static GameObject CreateCylinderCollider() {
            GameObject gameObject = new GameObject("Cylinder");
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

            Link.Geometry.Cylinder cylinder = new Link.Geometry.Cylinder(0.5, 2); //Default unity cylinder sizes

            meshCollider.sharedMesh = CreateCylinderMesh(cylinder);
            meshCollider.convex = true;

            return gameObject;
        }
    }
}
