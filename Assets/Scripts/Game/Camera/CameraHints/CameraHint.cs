using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Benco.Camera
{
    /// <summary>
    /// The base class for all camera controllers. The class returns whether or not an gamatography issue has been
    /// detected is IsDetected(), and fixes the issue in OperateCamera().
    /// </summary>
    public abstract class CameraHint
    {
        public PrincipalAxes principalAxesPriority = PrincipalAxes.Size;
        public FieldOfView fieldOfViewPriority = FieldOfView.Size;
        public CameraOffsetPriority cameraOffsetPriority = CameraOffsetPriority.Size;

        public bool dirty = true;
        private bool detected = false;

        /// <summary>
        /// A message to the programmer that reminds them to set the priority for a CameraHint for each 
        /// DegreeOfFreedomGroup.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DebugWarnings()
        {
            if (principalAxesPriority == PrincipalAxes.Size)
            {
                Debug.LogErrorFormat("Class {0} did not set the priority for principal axes.", GetType());
            }
            if (fieldOfViewPriority == FieldOfView.Size)
            {
                Debug.LogErrorFormat("Class {0} did not set the priority for field of view.", GetType());
            }
            if (cameraOffsetPriority == CameraOffsetPriority.Size)
            {
                Debug.LogErrorFormat("Class {0} did not set the priority for camera offset.", GetType());
            }
        }
        
        public CameraHint(bool enabled = false)
        {
            DebugWarnings();
            _enabled = enabled;
        }

        private bool _enabled = false;
        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                Debug.Log("_enabled: " + _enabled);
                if (_enabled)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        /// <summary>
        /// Called once per frame when enabled is true.
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// Called only when enabled is set to true.
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        /// Called only when enabled is set to false.
        /// </summary>
        protected virtual void OnDisable() { }
        
        /// <returns>
        /// True if the CameraHint has detected a problem and would like control over the camera's movement.
        /// </returns>
        public bool IsTriggered()
        {
            if (_enabled)
            {
                if (dirty)
                {
                    Update();
                    detected = IsDetected();
                }
                return detected;
            }
            return false;
        }

        protected abstract bool IsDetected();

        public abstract void OperateCamera(Transform camTransform,
                                           CinemachineVirtualCamera camera,
                                           PriorityGroup permissions);
    }
}
