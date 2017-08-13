using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Benco.Camera
{
    [System.Flags]
    public enum PriorityGroup
    {
        PrincipalAxes = 1 << 0,
        CameraOffset = 1 << 1,
        FieldOfView = 1 << 2,
        All = PrincipalAxes | CameraOffset | FieldOfView
    }

    /// <summary>
    /// A List of CameraHints sorted in order by their priority for a given DegreeOfFreedomGroup.
    /// </summary>
    public class CameraPriorityGroup
    {
        /// <summary>
        /// An array of CameraHints ordered by priority.
        /// </summary>
        protected CameraHint[] cameraHints;
        public CameraPriorityGroup()
        {

        }

        //TODO(mderu): Remove this after the priority sort and instance fetching has been implemented.
        public void SetCameraHints(CameraHint[] cameraHints)
        {
            this.cameraHints = cameraHints;
        }

        /// <summary>
        /// Returns the current highest priority <see cref="CameraHint"/> that has been triggered.
        /// </summary>
        /// <param name="camTransform"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public CameraHint GetCameraHint(Transform camTransform, CinemachineVirtualCamera camera)
        {
            cameraHints.AssertNotEmpty();
            for (int i = 0; i < cameraHints.Length; i++)
            {
                if (cameraHints[i].IsTriggered())
                {
                    return cameraHints[i];
                }
            }
            return null;
        }
    }
}
