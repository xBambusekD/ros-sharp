using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public class UrdfGeometryVisualRuntime : UrdfGeometryRuntime {
        public static void Create(Transform parent, GeometryTypes geometryType, Link.Geometry geometry = null, bool useColliderInVisuals = false) {
            GameObject geometryGameObject = null;

            switch (geometryType) {
                case GeometryTypes.Box:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    geometryGameObject.transform.DestroyImmediateIfExists<BoxCollider>();
                    break;
                case GeometryTypes.Cylinder:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    geometryGameObject.transform.DestroyImmediateIfExists<CapsuleCollider>();
                    break;
                case GeometryTypes.Sphere:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    geometryGameObject.transform.DestroyImmediateIfExists<SphereCollider>();
                    break;
                case GeometryTypes.Mesh:
                    if (geometry != null)
                        geometryGameObject = CreateMeshVisual(geometry.mesh, useColliderInVisuals);
                    //else, let user add their own mesh gameObject
                    break;
            }

            if (geometryGameObject != null) {
                geometryGameObject.transform.SetParentAndAlign(parent);
                if (geometry != null)
                    SetScale(parent, geometry, geometryType);
            }
        }

        private static GameObject CreateMeshVisual(Link.Geometry.Mesh mesh, bool useColliderInVisuals = false) {
            GameObject meshObject = UrdfAssetImporterRuntime.Instance.ImportUrdfAsset(mesh.filename, useColliderInVisuals);
            return meshObject;
        }
    }
}
