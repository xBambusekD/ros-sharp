using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf.Runtime {
    public static class UrdfLinkExtensionsRuntime {
        public static UrdfLink Create(Transform parent, Link link = null, Joint joint = null, bool useColliderInVisuals = false, bool useUrdfMaterials = false) {
            GameObject linkObject = new GameObject("link");
            linkObject.transform.SetParentAndAlign(parent);
            UrdfLink urdfLink = linkObject.AddComponent<UrdfLink>();

            // If useColliderInVisuals is set to true, colliders will be added as duplicates of MeshRenderer into Visuals
            UrdfVisualsExtensionsRuntime.Create(linkObject.transform, link?.visuals, useColliderInVisuals:useColliderInVisuals, useUrdfMaterials:useUrdfMaterials);

            // Import colliders into Collisions GameObject only if useColliderInVisuals set to false,
            // otherwise colliders will be imported as duplication of MeshRenderer in Visuals
            if (!useColliderInVisuals) {
                UrdfCollisionsExtensionsRuntime.Create(linkObject.transform, link?.collisions);
            }

            if (link != null)
                urdfLink.ImportLinkData(link, joint, useColliderInVisuals, useUrdfMaterials);
            else {
                UrdfInertial.Create(linkObject);
            }

            return urdfLink;
        }

        private static void ImportLinkData(this UrdfLink urdfLink, Link link, Joint joint, bool useColliderInVisuals = false, bool useUrdfMaterials = false) {
            if (link.inertial == null && joint == null)
                urdfLink.IsBaseLink = true;

            urdfLink.gameObject.name = link.name;

            if (joint?.origin != null)
                UrdfOrigin.ImportOriginData(urdfLink.transform, joint.origin);

            if (link.inertial != null) {
                UrdfInertial.Create(urdfLink.gameObject, link.inertial);

                if (joint != null)
                    UrdfJoint.Create(urdfLink.gameObject, UrdfJoint.GetJointType(joint.type), joint);
            } else if (joint != null)
                Debug.LogWarning("No Joint Component will be created in GameObject \"" + urdfLink.gameObject.name + "\" as it has no Rigidbody Component.\n"
                                 + "Please define an Inertial for Link \"" + link.name + "\" in the URDF file to create a Rigidbody Component.\n", urdfLink.gameObject);

            foreach (Joint childJoint in link.joints) {
                Link child = childJoint.ChildLink;
                UrdfLinkExtensionsRuntime.Create(urdfLink.transform, child, childJoint, useColliderInVisuals, useUrdfMaterials);
            }
        }
    }
}
