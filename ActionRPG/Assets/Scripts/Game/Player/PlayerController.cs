using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Managers;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonobehaviourExtension
    {
        [Header("Stats")]
        [SerializeField] private float sprintMultiplier = 3.0f;
        [SerializeField] float jumpPower = 12f;
        [SerializeField] private float moveSpeedMultiplier = 1.0f;
        [SerializeField] private float inAirAccelerationMultiplier;
        [SerializeField] private float animSpeedMultiplier = 1.0f;
        [Range(1f, 4f), SerializeField] float gravityMultiplier = 2f;

        private Animator anim;
        private CharacterController controller;
        private Transform cam;
        private float deltaTime;

        private Vector3 move;
        private bool jump;
        private bool grounded;

        public bool disableMove { get; set; }

        private static class AnimParams
        {
            public static readonly int moveSpeed = Animator.StringToHash("MoveSpeed");
        }

        private void Start()
        {
            InitializeUpdateFlags(false, true, false);

            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Player has no Animator", this);
            }

            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("Player has no CharacterController", this);
            }
        }

        protected override void MyFixedUpdate()
        {
            // set necessary cached fields
            deltaTime = Time.deltaTime;

            // handle input to set move and jump parameters
            HandleInput();

            // pass all parameters to the move function
            Move();
        }

        //private void FixedUpdate()
        //{
        //    // set necessary cached fields
        //    deltaTime = Time.deltaTime;

        //    // handle input to set move and jump parameters
        //    HandleInput();

        //    // pass all parameters to the move function
        //    Move();
        //}

        private void HandleInput()
        {
            float horizontal = InputManager.Move.x.value;
            float vertical = InputManager.Move.y.value;
            jump = InputManager.Jump.wasPressed;
            move = Vector3.zero;

            // calculate move direction to pass to character
            if (cam != null)
            {
                // calculate camera relative direction to move:
                Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                move = vertical * camForward + horizontal * cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                move = vertical * Vector3.forward + horizontal * Vector3.right;
            }

            // run speed multiplier
            //if (InputManager.input.sprint.isPressed)
            //{
            //    // move *= walkMultiplier;
            //    moveSpeedMultiplier = sprintMultiplier;
            //}
            //else
            //{
            //    moveSpeedMultiplier = 1;
            //}
        }

        public void Move()
        {
            // don't move if disabled
            if (disableMove)
            {
                return;
            }

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();

            // rotate model toward its movement vector
            if (move != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Scale(Camera.main.transform.TransformDirection(move), new Vector3(1, 0, 1)));
            }

            //move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            // control and velocity handling is different when grounded and airborne:
            if (controller.isGrounded)
            {
                HandleGroundedMovement();
            }
            else
            {
                HandleAirborneMovement();
            }

            // send input and other state parameters to the animator
            UpdateAnimator();
        }

        void UpdateAnimator()
        {
            move = transform.InverseTransformDirection(move);
            // update the animator parameters
            anim.SetFloat("Speed", move.magnitude, 0.1f, deltaTime);
            grounded = controller.isGrounded;
            anim.SetBool("Grounded", grounded);

            if (!grounded)
            {
                anim.SetFloat("Jump", controller.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            //float runCycle =
            //    Mathf.Repeat(
            //        anim.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            //float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            //if (grounded)
            //{
            //    anim.SetFloat("JumpLeg", jumpLeg);
            //}

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (grounded && move.magnitude > 0)
            {
                anim.speed = animSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                anim.speed = 1;
            }
        }

        void HandleAirborneMovement()
        {
            // air movement animations don't control transform
            //anim.applyRootMotion = false;
            // apply extra gravity from multiplier:
            //Vector3 extraGravityForce = ((Physics.gravity * gravityMultiplier) - Physics.gravity) * deltaTime;
            //extraGravityForce.y += controller.velocity.y;
            //move = Vector3.Scale(Camera.main.transform.TransformDirection(move), new Vector3(1, 0, 1));
            //move = move.normalized;
            //controller.Move((inAirAccelerationMultiplier * move + extraGravityForce) * deltaTime);
            //controller.Move((controller.velocity + extraGravityForce) * Time.deltaTime);
            //controller.Move(extraGravityForce * deltaTime);
            //if (move == Vector3.zero)
            //{
            //    anim.applyRootMotion = false;
            //}
            //else
            //{
            //    anim.applyRootMotion = false;
            //}
        }


        void HandleGroundedMovement()
        {
            // ground movement animation controls transform
            //anim.applyRootMotion = true;
            // check whether conditions are right to allow a jump:
            //if (jump && anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            //{
            //    // jump!
            //    controller.Move((new Vector3(controller.velocity.x, jumpPower, controller.velocity.z)) * deltaTime);
            //}
        }

        public void OnAnimatorMove()
        {
            deltaTime = Time.deltaTime;
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (deltaTime > 0)
            {
                Vector3 v = (anim.deltaPosition * moveSpeedMultiplier) / deltaTime;
                // we preserve the existing y part of the current velocity.
                v.y = controller.velocity.y;
                v += ((Physics.gravity * gravityMultiplier) - Physics.gravity) * deltaTime;
                if (jump && grounded)
                {
                    v.y = jumpPower;
                }
                if (!grounded && move == Vector3.zero)
                {
                    v = Vector3.Scale(v, new Vector3(0, 1, 0));
                }
                controller.Move(v * deltaTime);
           }
        }
    }
}
