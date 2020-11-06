using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {

    /// <summary>
    /// Used when model is imported.
    /// </summary>
    public class ImportedModelEventArgs : EventArgs {
        /// <summary>
        /// Imported GameObject.
        /// </summary>
        public GameObject RootGameObject {
            get; set;
        }

        /// <summary>
        /// If true, Colliders are included along with Visuals.
        /// </summary>
        public bool CollidersIncludedWithVisuals {
            get; set;
        }

        /// <summary>
        /// If true, RootGameObject has Colliders only. MeshFilters and MeshRenderers are deleted.
        /// </summary>
        public bool CollidersOnly {
            get; set;
        }

        public ImportedModelEventArgs(GameObject gameObject, bool collidersIncludedWithVisuals, bool collidersOnly) {
            RootGameObject = gameObject;
            CollidersIncludedWithVisuals = collidersIncludedWithVisuals;
            CollidersOnly = collidersOnly;
        }
    }

    public class UrdfAssetImporterRuntime : MonoBehaviour {
        #region SINGLETON
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static UrdfAssetImporterRuntime m_Instance;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static UrdfAssetImporterRuntime Instance {
            get {
                if (m_ShuttingDown) {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(UrdfAssetImporterRuntime) +
                        "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock) {
                    if (m_Instance == null) {
                        // Search for existing instance.
                        m_Instance = (UrdfAssetImporterRuntime) FindObjectOfType(typeof(UrdfAssetImporterRuntime));

                        // Create new instance if one doesn't already exist.
                        if (m_Instance == null) {
                            // Need to create a new GameObject to attach the singleton to.
                            GameObject singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<UrdfAssetImporterRuntime>();
                            singletonObject.name = typeof(UrdfAssetImporterRuntime).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        private void OnApplicationQuit() {
            m_ShuttingDown = true;
        }


        private void OnDestroy() {
            m_ShuttingDown = true;
        }
        #endregion


        public delegate void ImportedModelEventHandler(object sender, ImportedModelEventArgs args);
        public event ImportedModelEventHandler OnModelImported;

        public string CurrentUrdfRoot {
            get;
            set;
        }

        /// <summary>
        /// Imports model of type DAE, FBX, OBJ, GLTF2, STL, PLY, 3MF into placeholder object, which is returned immediately.
        /// After the model itself is imported, the OnModelImported action is triggered.
        /// </summary>
        /// <param name="assetPath">Full path of the model to be imported.</param>
        /// <param name="useColliderInVisuals">Set to true, if colliders needs to be added directly to visuals. Meshes for colliders will be duplicated from MeshRenderer.</param>
        /// <param name="useCollidersOnly">Set to true, if importing model with colliders only (without MeshRenderers).</param>
        /// <returns></returns>
        public GameObject ImportUrdfAsset(string assetPath, bool useColliderInVisuals = false, bool useCollidersOnly = false) {
            if (!assetPath.StartsWith(@"package://")) {
                Debug.LogWarning(assetPath + " is not a valid URDF package file path. Path should start with \"package://\".");
                return null;
            }
            string path = assetPath.Substring(10);
            path = Path.Combine(CurrentUrdfRoot, path);

            GameObject loadedObject = new GameObject("ImportedMeshObject");

            if (Path.GetExtension(path).ToLower() == ".dae") {
                StreamReader reader = File.OpenText(path);
                string daeFile = reader.ReadToEnd();

                // Requires Simple Collada asset from Unity Asset Store: https://assetstore.unity.com/packages/tools/input-management/simple-collada-19579
                // Supports: DAE
                StartCoroutine(ColladaImporter.Instance.ImportAsync(daeFile, Quaternion.identity, Vector3.one, Vector3.zero,
                    onModelImported: delegate (GameObject loadedGameObject) {

                        // Add Colliders directly to Visuals.
                        if (useColliderInVisuals) {
                            AddColliders(loadedGameObject);
                        }
                        // Or remove Renderers and Colliders only.
                        else if (useCollidersOnly) {
                            AddColliders(loadedGameObject, useCollidersOnly:useCollidersOnly);
                        }

                        OnModelImported?.Invoke(this, new ImportedModelEventArgs(loadedGameObject, useColliderInVisuals, useCollidersOnly));
                    }, wrapperGameObject:loadedObject, includeEmptyNodes: true));

            } else {
                // Requires Trilib 2 asset from Unity Asset Store: https://assetstore.unity.com/packages/tools/modeling/trilib-2-model-loading-package-157548
                // Supports: FBX, OBJ, GLTF2, STL, PLY, 3MF
                AssetLoaderOptions assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                AssetLoader.LoadModelFromFile(path, null, delegate (AssetLoaderContext assetLoaderContext) {
                    assetLoaderContext.RootGameObject.transform.Rotate(0f, 180f, 0f);

                    // Add Colliders directly to Visuals.
                    if (useColliderInVisuals) {
                        AddColliders(assetLoaderContext.RootGameObject);
                    }
                    // Or remove Renderers and Colliders only.
                    else if (useCollidersOnly) {
                        AddColliders(assetLoaderContext.RootGameObject, useCollidersOnly:useCollidersOnly);
                    }

                    OnModelImported?.Invoke(this, new ImportedModelEventArgs(assetLoaderContext.RootGameObject, useColliderInVisuals, useCollidersOnly));
                }, null, assetLoaderOptions: assetLoaderOptions, wrapperGameObject: loadedObject);
            }
            
            return loadedObject;
        }

        /// <summary>
        /// Adds mesh colliders to every existing MeshFilter on specified gameObject.
        /// </summary>
        /// <param name="gameObject">GameObject to traverse childs and add colliders.</param>
        /// <param name="setConvex">Set to true, if colliders are to be convex.</param>
        /// <param name="useCollidersOnly">If set to true, MeshFilters and MeshRenderers will be deleted and only Colliders will stay.</param>
        private void AddColliders(GameObject gameObject, bool setConvex = false, bool useCollidersOnly = false) {
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters) {
                GameObject child = meshFilter.gameObject;
                MeshCollider meshCollider = child.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;

                meshCollider.convex = setConvex;

                if (useCollidersOnly) {
                    DestroyImmediate(child.GetComponent<MeshRenderer>());
                    DestroyImmediate(meshFilter);
                }
            }
        }
    }
}
