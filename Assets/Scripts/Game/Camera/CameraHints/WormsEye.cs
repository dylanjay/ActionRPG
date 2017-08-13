using Cinemachine;
using UnityEngine;

namespace Benco.Camera
{
    /// <summary>
    /// The effect that increases the field of view as the camera gets nearer to the ground.
    /// </summary>
    public class WormsEye : CameraHint
    {
        /// <summary>
        /// How smooth the transition from the old to new field of view values should be. Must be between 0 and 1.
        /// </summary>
        [Range(0, 1), SerializeField]
        private float smoothingFactor = .8f;

        public WormsEye(bool enabled = false) : base(enabled) { }
        
        protected override bool IsDetected()
        {
            return true;
        }

        public override void OperateCamera(Transform camTransform,
                                           CinemachineVirtualCamera camera,
                                           PriorityGroup permissions)
        {
            //TODO(mderu): Add slope detection. If the player is running up a 45 degree incline, the camera
            //             shouldn't increase the field of view if it is parallel to the slope.
            float rotation = camTransform.rotation.eulerAngles.x;
            if (rotation > 180) { rotation -= 360; }
            float strength = Mathf.Clamp01(rotation / -30.0f);
            
            camera.m_Lens.FieldOfView = Mathf.SmoothStep(Mathf.Lerp(40, 90, strength),
                                                         camera.m_Lens.FieldOfView,
                                                         smoothingFactor);
        }
    }
}
