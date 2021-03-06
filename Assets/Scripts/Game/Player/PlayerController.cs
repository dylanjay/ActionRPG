﻿using System.Collections;
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

        [Header("Refs")]
        public GameObject spike;

        // AttackSpike
        private Renderer spikeRenderer;

        // Model for MVC
        private PlayerModel model;

        // Components
        private CharacterController controller;

        // Cached Variables
        private Transform mainCamera;
        private float deltaTime;

        // Disable Input
        private bool disableInput = false;

        // Movement Variables
        private Vector2 lastMoveInput = Vector3.zero;
        private bool newMoveInput = false;
        private Vector3 lastMoveVector = Vector3.zero;

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
        private Attack attackIndex = Attack.LeftPunch;
        private Timer comboResetTimer;

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
            spikeRenderer = spike.GetComponent<Renderer>();
            comboResetTimer = MonoBehaviourManager.Instance.AddTimer(model.comboResetTime);
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
                newMoveInput = false;
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
                    StartCoroutine(model.lockOnTrigger.Set());
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

            // check if we are getting a new input direction
            Vector2 input = new Vector2(horizontal, vertical);
            if (input != Vector2.zero && input != lastMoveInput)
            {
                newMoveInput = true;
            }
            lastMoveInput = input;

            //model.moveVector = newMoveInput || input == Vector3.zero ? Vector3.zero : lastMoveVector;

            // calculate move direction to pass to character
            if (mainCamera != null)
            {
                Vector3 camForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                //model.moveVector = vertical * camForward + horizontal * mainCamera.right;
                model.moveVector = NormalizeMoveInput(vertical) * camForward + NormalizeMoveInput(horizontal) * mainCamera.right;
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
            // Temporary Combo
            attack = InputManager.attack.wasPressed;
            if (attack && model.isAttacking)
            {
                attack = false;
            }

            if (comboResetTimer.isTriggered)
            {
                attackIndex = Attack.LeftPunch;
            }

            //attack = InputManager.attack.wasPressed || InputManager.rightAttack.wasPressed;
            //if (InputManager.attackModifier.isPressed)
            //{
            //    if (InputManager.attack.wasPressed)
            //    {
            //        attackIndex = Attack.LeftKick;
            //    }
            //    else if (InputManager.rightAttack.wasPressed)
            //    {
            //        attackIndex = Attack.RightKick;
            //    }
            //}
            //else
            //{
            //    if (InputManager.attack.wasPressed)
            //    {
            //        attackIndex = Attack.LeftPunch;
            //    }
            //    else if (InputManager.rightAttack.wasPressed)
            //    {
            //        attackIndex = Attack.RightPunch;
            //    }
            //}
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
                StartCoroutine(model.landTrigger.Set());
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

        private int NormalizeMoveInput(float value)
        {
            if (value < 0)
            {
                return -1;
            }
            if (value > 0)
            {
                return 1;
            }
            return 0;
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
            Vector3 moveVector2D = model.moveVector.xz();
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
            model.moveVector += extraGravityForce * (model.isFalling ? model.fallingGravityMultiplier : 1f);

            // check if falling
            if (model.currentState != State.Grounded && model.currentState != State.Falling && controller.velocity.y <= model.fallingVelocity)
            {
                model.ChangeState(State.Falling);
                StartCoroutine(model.fallTrigger.Set());
            }

            // clamp X / Z to limit velocity we can achieve
            //model.moveVector = new Vector3(Mathf.Clamp(model.moveVector.x, model.minAirVelocity, model.maxAirVelocity), model.moveVector.y,
            //                                  Mathf.Clamp(model.moveVector.z, model.minAirVelocity, model.maxAirVelocity));

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
            StartCoroutine(model.jumpTrigger.Set());
            model.moveVector.y = model.jumpForce;
        }

        public IEnumerator AttackMove()
        {
            model.ChangeState(State.Attacking);
            StartCoroutine(model.attackTrigger.Set());
            StartCoroutine(model.rootMotionTrigger.Set(true));
            model.attackIndex = attackIndex;
            Vector3 centerPlayerPos = transform.position;
            switch (attackIndex)
            {
                case Attack.LeftPunch:
                    StartCoroutine(ThrustSpike(centerPlayerPos + 2f * transform.forward + -transform.right / 2f, 
                                   centerPlayerPos + 3.5f * transform.forward + (Vector3.up * controller.height / 2)
                                   + transform.right / 2f, 0.5f, 0.2f, 0.1f));
                    yield return StartCoroutine(LockPlayerInputForSeconds(0, .5f));
                    break;
                case Attack.RightPunch:
                    StartCoroutine(ThrustSpike(centerPlayerPos + 2f * transform.forward + transform.right / 2f,
                                   centerPlayerPos + 3.5f * transform.forward + (Vector3.up * controller.height / 2)
                                   + -transform.right / 2f, 0.5f, 0.2f, 0.1f));
                    yield return StartCoroutine(LockPlayerInputForSeconds(0, .5f));
                    break;
                case Attack.LeftKick:
                    StartCoroutine(ThrustSpike(centerPlayerPos + 2f * transform.forward, centerPlayerPos + 3.5f * transform.forward + 
                                  (Vector3.up * controller.height / 2), 0.7f, 0.3f, 0.1f));
                    yield return StartCoroutine(LockPlayerInputForSeconds(0, .7f));
                    break;
            }
            model.ChangeState(model.lastState);
            StartCoroutine(model.rootMotionTrigger.Set(false));
            comboResetTimer.active = true;
            if (attackIndex == Attack.LeftKick)
            {
                attackIndex = Attack.LeftPunch;
            }
            else
            {
                attackIndex++;
            }
            comboResetTimer.Reset();
        }

        public IEnumerator Stagger()
        {
            model.ChangeState(State.Staggering);
            StartCoroutine(model.staggerTrigger.Set());

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
            StartCoroutine(model.dieTrigger.Set());
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, 1.5f));
        }

        private IEnumerator Revive()
        {
            model.ChangeState(model.lastState);
            StartCoroutine(model.reviveTrigger.Set());
            yield return StartCoroutine(LockPlayerInputForSeconds(0f, 0.5f));
        }

        private IEnumerator ThrustSpike(Vector3 startPos, Vector3 endPos, float totalTime, float delayTime, float recedeTime)
        {
            
            spike.transform.position = startPos;
            spike.transform.up = Vector3.Normalize(endPos - startPos);
            float elapsedTime = 0f;
            float startRecede = totalTime - recedeTime;
            float activeTime = 0f;
            while (elapsedTime < startRecede)
            {
                if (elapsedTime >= delayTime)
                {
                    if (!spike.activeInHierarchy)
                    {
                        spike.SetActive(true);
                    }
                    spike.transform.position = Vector3.Lerp(startPos, endPos, activeTime / (startRecede - delayTime));
                    //spikeRenderer.material.SetFloat("_Cutoff", Mathf.Lerp(0.875f, 0.5f, elapsedTime / time));
                    activeTime += deltaTime;
                }
                elapsedTime += deltaTime;
                yield return new WaitForEndOfFrame();
            }

            while (elapsedTime < totalTime)
            {
                spike.transform.position = Vector3.Lerp(endPos, startPos, (elapsedTime - startRecede) / recedeTime);
                elapsedTime += deltaTime;
                yield return new WaitForEndOfFrame();
            }
            spike.SetActive(false);
        }

        private IEnumerator Roll()
        {
            model.ChangeState(State.Dashing);
            StartCoroutine(model.rollTrigger.Set());
            StartCoroutine(model.rootMotionTrigger.Set(true));
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
            StartCoroutine(model.rootMotionTrigger.Set(false));
        }

        #endregion
    }
}
