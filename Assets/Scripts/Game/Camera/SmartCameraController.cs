using BenCo.Framework;
using Benco.Camera;
using Cinemachine;

/// <summary>
/// The Controller for the CameraHint system, as well as other various camera effects.
/// </summary>
public class SmartCameraController : MonoBehaviourWrapper
{
    CameraPriorityGroup cameraPriorityGroup = new CameraPriorityGroup();

    void Start()
    {
        cameraPriorityGroup.SetCameraHints(new CameraHint[] { new WormsEye(enabled:true) });
    }

    void Update()
    {
        CameraHint cameraHint = cameraPriorityGroup.GetCameraHint(transform, GetComponent<CinemachineVirtualCamera>());
        cameraHint.OperateCamera(transform, GetComponent<CinemachineVirtualCamera>(), PriorityGroup.All);
    }
}
