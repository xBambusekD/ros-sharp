using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public class UrdfGeometryCollisionRuntime : UrdfGeometryRuntime {
        public static void Create(Transform parent, GeometryTypes geometryType, Link.Geometry geometry = null, Link.Geometry visual = null) {
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
                        geometryGameObject = CreateMeshCollider(geometry.mesh, visual?.mesh);
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

        private static GameObject CreateMeshCollider(Link.Geometry.Mesh mesh, Link.Geometry.Mesh visual) {

            // Use meshes from runtime imported DAE files
            //GameObject meshObject = UrdfAssetImporterRuntime.Instance.ImportUrdfAsset(visual.filename);
            GameObject meshObject = null;

            if (meshObject != null) {
                ConvertMeshToColliders(meshObject);
            }

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

        public static void CreateMatchingMeshCollision(Transform parent, Transform visualToCopy) {
            //if (visualToCopy.childCount == 0)
            //    return;

            //GameObject objectToCopy = visualToCopy.GetChild(0).gameObject;
            //GameObject prefabObject = (GameObject) PrefabUtility.GetCorrespondingObjectFromSource(objectToCopy);

            //GameObject collisionObject;
            //if (prefabObject != null)
            //    collisionObject = (GameObject) PrefabUtility.InstantiatePrefab(prefabObject);
            //else
            //    collisionObject = Object.Instantiate(objectToCopy);

            //collisionObject.name = objectToCopy.name;
            //ConvertMeshToColliders(collisionObject, true);

            //collisionObject.transform.SetParentAndAlign(parent);
        }

        private static void ConvertMeshToColliders(GameObject gameObject, bool setConvex = false) {
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters) {
                GameObject child = meshFilter.gameObject;
                MeshCollider meshCollider = child.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;

                meshCollider.convex = setConvex;

                Object.DestroyImmediate(child.GetComponent<MeshRenderer>());
                Object.DestroyImmediate(meshFilter);
            }
        }
    }
}
