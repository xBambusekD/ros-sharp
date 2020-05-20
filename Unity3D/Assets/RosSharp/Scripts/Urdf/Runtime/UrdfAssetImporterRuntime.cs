using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    /// <summary>
    /// Imports mesh in DAE format. Requires Simple Collada asset from Unity Asset Store:
    /// https://assetstore.unity.com/packages/tools/input-management/simple-collada-19579
    /// </summary>
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
        

        public string CurrentUrdfRoot {
            get;
            set;
        }

        /// <summary>
        /// Imports corresponding .dae file into Unity in runtime.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public GameObject ImportUrdfAsset(string assetPath) {
            if (!assetPath.StartsWith(@"package://")) {
                Debug.LogWarning(assetPath + " is not a valid URDF package file path. Path should start with \"package://\".");
                return null;
            }
            string path = assetPath.Substring(10);
            path = Path.Combine(CurrentUrdfRoot, path);

            if (Path.GetExtension(path).ToLower() == ".dae") {
                StreamReader reader = File.OpenText(path);
                string daeFile = reader.ReadToEnd();
                GameObject loadedObject = new GameObject();

                // Requires Simple Collada asset from Unity Asset Store: https://assetstore.unity.com/packages/tools/input-management/simple-collada-19579
                StartCoroutine(ColladaImporter.Instance.ImportAsync(loadedObject, daeFile, Quaternion.identity, new Vector3(1, 1, 1), Vector3.zero, includeEmptyNodes: true));

                return loadedObject;
            }

            return null;
        }
    }
}
