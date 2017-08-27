using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Framework;
using BenCo.Managers;
using Cinemachine;

namespace BenCo.BCamera
{
    [RequireComponent(typeof(CinemachineFreeLook))]
	public class PlayerCameraController : MonoBehaviourWrapper 
	{
        [SerializeField]
        public float horizontalRotateSpeed;

        private CinemachineFreeLook cam;

        private float deltaTime;

        private void Start()
        {
            InitializeUpdateFlags(true, false, false);
            cam = GetComponent<CinemachineFreeLook>();
        }

        protected override void MyUpdate()
        {
            deltaTime = MonoBehaviourManager.Instance.deltaTime;
            if (InputManager.move.x.value < 0)
            {
                cam.m_XAxis.Value -= horizontalRotateSpeed / (InputManager.move.y.value != 0 ? 2 : 1) * deltaTime;
            }
            else if (InputManager.move.x.value > 0)
            {
                cam.m_XAxis.Value += horizontalRotateSpeed / (InputManager.move.y.value != 0 ? 2 : 1) * deltaTime;
            }
        }
    }
}