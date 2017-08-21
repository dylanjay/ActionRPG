using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Framework;
using BenCo.Extensions;
using BenCo.Managers;

namespace BenCo.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviourWrapper
    {
        #region Variables

        // Model for MVC
        private PlayerModel model;

        // Components
        private CharacterController controller;

        // Cached Variables
        private Transform mainCamera;
        private float deltaTime;

        // Disable Input
        private bool disableInput = false;

        // Jumping Variables
        private bool jump = false;

        // Rolling Variables
        private bool roll = false;

        // Player Damaged Variables
        private bool stagger = false;
        private bool die = false;
        private bool revive = false;

        // Attack Variables
        private bool attack = false;
        private Attack attackIndex;

        // Lock On Variables
        private GameObject lockOnTarget;

        #endregion

        #region Initialization

        private void Start()
        {
            InitializeUpdateFlags(true, false, false);

            model = GetComponent<PlayerModel>();
            if (!model)
            {
                Debug.LogErrorFormat("{0} has no Player Model", this);
            }

            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("Player has no CharacterController", this);
            }

            mainCamera = Camera.main.transform;
            if (mainCamera == null)
            {
                Debug.LogError("Scene has no camera", this);
            }

            // TEMP
            lockOnTarget = GameObject.Find("TestEnemy");
        }

        #endregion

        #region Updates

        protected override void MyUpdate()
        {
            // Set necessary cached fields
            UpdateCachedVariables();

            // Check Grounded
            CheckGrounded();

            // Call all input handlers
            HandleInput();

            // Check Actions
            if (model.canAction)
            {
                // check grounded actions
                if (model.grounded)
                {
                    if (roll)
                    {
                        StartCoroutine(Roll());
                    }
                    if (jump && model.canJump)
                    {
                        Jump();
                    }
                    if (stagger)
                    {
                        StartCoroutine(Stagger());
                    }
                    if (die)
                    {
                        StartCoroutine(Die());
                    }
                    if (attack)
                    {
                        StartCoroutine(AttackMove());
                    }
                }
            }

            if (model.isDead && revive)
            {
                StartCoroutine(Revive());
            }

            if (model.canMove)
            {
                // Pass all parameters to the move function
                Move();
            }

            // Handle rotation
            if (model.lockOn && lockOnTarget)
            {
                RotateTowardsLockOnTarget();
            }
            else
            {
                RotateTowardsMovementDir();
            }
        }

        #endregion

        #region InputHandlers

        private void HandleInput()
        {
            // Check if player is alive
            if (!disableInput && !model.isDead)
            {
                // Handle input to set move
                HandleMovementInput();
                jump = InputManager.jump.wasPressed;

                HandleAttackingInput();
                roll = InputManager.dash.wasPressed;
                die = InputManager.die.wasPressed;
                revive = false;
                stagger = InputManager.stagger.wasPressed;

                if (InputManager.lockOn.wasPressed)
                {
                    model.lockOn = !model.lockOn;
                    model.lockOnTrigger.Set();
                }
            }
            else
            {
                revive = InputManager.die.wasPressed;
                die = false;
            }
        }

        private void HandleMovementInput()
        {
            float horizontal = InputManager.move.x.value;
            float vertical = InputManager.move.y.value;
            model.moveVector = Vector3.zero;

            // calculate move direction to pass to character
            if (mainCamera != null)
            {
                // calculate camera relative direction to move:
                Vector3 camForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                model.moveVector = vertical * camForward + horizontal * mainCamera.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                model.moveVector = vertical * Vector3.forward + horizontal * Vector3.right;
            }

            if (!model.isRolling)
            {
                model.targetDashDirection = model.moveVector;
            }
        }

        private void HandleAttackingInput()
        {
            attack = InputManager.leftAttack.wasPressed || InputManager.rightAttack.wasPressed;
            if (InputManager.attackModifier.isPressed)
            {
                if (InputManager.leftAttack.wasPressed)
                {
                    attackIndex = Attack.LeftKick;
                }
                else if (InputManager.rightAttack.wasPressed)
                {
                    attackIndex = Attack.RightKick;
                }
            }
            else
            {
                if (InputManager.leftAttack.wasPressed)
                {
                    attackIndex = Attack.LeftPunch;
                }
                else if (InputManager.rightAttack.wasPressed)
                {
                    attackIndex = Attack.RightPunch;
                }
            }

            //leftPunch = InputManager.leftPunch.wasPressed;
            //rightPunch = InputManager.punchRight.wasPressed;
            //leftKick = InputManager.kickLeft.wasPressed;
            //rightKick = InputManager.kickRight.wasPressed;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Block user input from interacting with the character
        /// </summary>
        /// <param name="active"></param>
        public void LockPlayerInput(bool active)
        {
            disableInput = active;
        }

        #endregion

        #region Helpers

        private void UpdateCachedVariables()
        {
            deltaTime = MonoBehaviourManager.Instance.deltaTime;
            model.currentVelocity = controller.velocity;
        }

        private void CheckGrounded()
        {
            if (!model.grounded && controller.isGrounded && model.canAction)
            {
                model.ChangeState(State.Grounded);
                model.landTrigger.Set();
            }
            model.grounded = controller.isGrounded;
        }

        private IEnumerator LockPlayerInputForSeconds(float startLock, float endLock)
        {
            yield return new WaitForSeconds(startLock);
            disableInput = true;
            yield return new WaitForSeconds(endLock);
            disableInput = false;
        }

        #endregion

        #region Actions

        public void Move()
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            //if (model.moveVector.magnitude > 1f) model.moveVector.Normalize();

            //control and velocity handling is different when grounded and airborne:
            if (model.grounded)
            {
                HandleGroundedMovement();
            }
            else
            {
                HandleAirborneMovement();
            }

            controller.Move(model.moveVector * deltaTime);
        }

        private void RotateTowardsLockOnTarget()
        {
            //make character point at target
            Quaternion targetRotation;
            Vector3 targetPos = lockOnTarget.transform.position;
            targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (model.turnSpeed * Time.deltaTime) * model.turnSpeed);
        }

        private void RotateTowardsMovementDir()
        {
            // rotate model toward its movement vector
            Vector3 moveVector2D = Vector3.Scale(model.moveVector, new Vector3(1, 0, 1));
            if (moveVector2D != Vector3.zero && !model.lockOn && !model.isRolling)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector2D), model.turnSpeed * deltaTime);
            }
        }

        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = ((Physics.gravity * model.gravityMultiplier) - Physics.gravity) * deltaTime;
            extraGravityForce.y += controller.velocity.y;
            model.moveVector *= model.airbornSpeed;
            model.moveVector += extraGravityForce;

            // check if falling
            if (model.currentState != State.Grounded && model.currentState != State.Falling && controller.velocity.y < model.fallingVelocity)
            {
                model.ChangeState(State.Falling);
                model.fallTrigger.Set();
            }

            // clamp X / Z to limit velocity we can achieve
            model.moveVector = new Vector3(Mathf.Clamp(model.moveVector.x, model.minAirVelocity, model.maxAirVelocity), model.moveVector.y,
                                              Mathf.Clamp(model.moveVector.z, model.minAirVelocity, model.maxAirVelocity));

        }

        void HandleGroundedMovement()
        {
            Vector3 moveVector = model.moveVector.xz();

            // if rolling use rolling speed else use move speed for xz movement
            moveVector *= model.isRolling ? model.rollSpeed : model.moveSpeed;

            // pipe through y component and set move vector
            moveVector.y = model.moveVector.y;
            model.moveVector = moveVector;

            // apply gravity if player isn't jumping
            if (!model.isJumping)
            {
                Vector3 extraGravityForce = ((Physics.gravity * model.gravityMultiplier) - Physics.gravity) * deltaTime;
                extraGravityForce.y += controller.velocity.y;
                model.moveVector += extraGravityForce;
            }
        }

        private void Jump()
        {
            model.ChangeState(State.Jumping);
            model.jumpTrigger.Set();
            model.moveVector.y = model.jumpForce;
        }

        public IEnumerator AttackMove()
        {
            model.ChangeState(State.Attacking);
            model.attackTrigger.Set();
            model.rootMotionTrigger.Set(true);
            model.attackIndex = attackIndex;
            yield return StartCoroutine(LockPlayerInputForSeconds(0, .8f));
            model.ChangeState(model.lastState);
            model.rootMotionTrigger.Set(false);
        }

        public IEnumerator Stagger()
        {
            model.ChangeState(State.Staggering);
            model.staggerTrigger.Set();

            int hits = 5;
            int hitNumber = Random.Range(0, hits);

            //apply directional knockback force
            if (hitNumber <= 1)
            {
                StartCoroutine(_Knockback(-transform.forward, 4, 2));
                model.staggerDirection = StaggerDirection.Backward;
            }
            else if (hitNumber == 2)
            {
                StartCoroutine(_Knockback(transform.forward, 4, 2));
                model.staggerDirection = StaggerDirection.Forward;
            }
            else if (hitNumber == 3)
            {
                StartCoroutine(_Knockback(transform.right, 4, 2));
                model.staggerDirection = StaggerDirection.Right;
            }
            else if (hitNumber == 4)
            {
                StartCoroutine(_Knockback(-transform.right, 4, 2));
                model.staggerDirection = StaggerDirection.Left;
            }

            yield return StartCoroutine(LockPlayerInputForSeconds(.1f, .4f));

            model.ChangeState(model.lastState);
        }

        IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            controller.Move(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (model.knockbackMultiplier * 10)) * deltaTime);
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, 0.1f));
        }

        IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            while (model.isStaggering)
            {
                controller.Move(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (model.knockbackMultiplier * 10)) * deltaTime);
                yield return null;
            }
        }

        private IEnumerator Die()
        {
            model.ChangeState(State.Dead);
            model.dieTrigger.Set();
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, 1.5f));
        }

        private IEnumerator Revive()
        {
            model.ChangeState(model.lastState);
            model.reviveTrigger.Set();
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, 0.5f));
        }

        private IEnumerator Roll()
        {
            model.ChangeState(State.Dashing);
            model.rollTrigger.Set();
            model.rootMotionTrigger.Set(true);
            if (model.lockOn && model.targetDashDirection != Vector3.zero)
            {
                //check which way the dash is pressed relative to the character facing
                float angle = Vector3.Angle(model.targetDashDirection, -transform.forward);
                float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(model.targetDashDirection, transform.forward)));
                // angle in [-179,180]
                float signed_angle = angle * sign;
                //angle in 0-360
                float angle360 = (signed_angle + 180) % 360;
                //deternime the animation to play based on the angle
                if (angle360 > 315 || angle360 < 45)
                {
                    model.dashDirection = DashDirection.Forward;
                }
                if (angle360 > 45 && angle360 < 135)
                {
                    model.dashDirection = DashDirection.Right;
                }
                if (angle360 > 135 && angle360 < 225)
                {
                    model.dashDirection = DashDirection.Backward;
                }
                if (angle360 > 225 && angle360 < 315)
                {
                    model.dashDirection = DashDirection.Left;
                }
            }
            else
            {
                model.dashDirection = DashDirection.Forward;
            }
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, model.rollDuration));
            model.ChangeState(model.lastState);
            model.rootMotionTrigger.Set(false);
        }

        #endregion
    }
}
