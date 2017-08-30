using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Framework;

namespace BenCo.Player
{
    public enum State
    {
        FullControl,

        Grounded,
        Jumping,
        DoubleJumping,
        Falling,

        LockInput,

        Dashing,
        Attacking,
        Staggering,
        Dead,

        Size
    }

    public enum DashDirection { Forward, Backward, Left, Right }
    public enum StaggerDirection { Forward, Backward, Left, Right }
    public enum Attack { LeftPunch, RightPunch, LeftKick, RightKick }

    public class PlayerModel : Model
    {
        public State currentState = State.Grounded;
        public State lastState = State.Grounded;

        // Tweakables
        [Header("Stats")]
        public float moveSpeed = 8f;
        public float airbornSpeed = 6f;
        public float turnSpeed = 40f;
        public float rollSpeed = 8f;
        public float rollDuration = 0.5f;
        public float jumpForce = 12f;
        public float doubleJumpForce = 12f;
        public float sprintMultiplier = 2f;
        public float moveSpeedMultiplier = 1f;
        public float walkSpeedMultiplier = 0.2f;
        public float knockbackMultiplier = 1f;
        public float animSpeedMultiplier = 1f;
        public float fallingGravityMultiplier = 1.05f;
        [Range(1f, 4f)] public float gravityMultiplier = 2f;
        public float comboResetTime = 3f;

        // Read Only State Variables
        public bool canMove { get { return currentState < State.LockInput; } }
        public bool canJump { get { return grounded && currentState < State.LockInput; } }
        public bool canDoubleJump { get { return !grounded && currentState < State.LockInput && jumpsUsed < maxJumps; } }
        public bool canAction { get { return currentState < State.LockInput; } }
        public bool isJumping { get { return currentState == State.Jumping; } }
        public bool isAttacking { get { return currentState == State.Attacking; } }
        public bool isStaggering { get { return currentState == State.Staggering; } }
        public bool isDoubleJumping { get { return currentState == State.DoubleJumping; } }
        public bool isFalling { get { return currentState == State.Falling; } }
        public bool isRolling { get { return currentState == State.Dashing; } }
        public bool isDead { get { return currentState == State.Dead; } }

        // State Triggers
        public Trigger rootMotionTrigger = new Trigger();
        public Trigger lockOnTrigger = new Trigger();
        public Trigger dieTrigger = new Trigger();
        public Trigger reviveTrigger = new Trigger();
        public Trigger jumpTrigger = new Trigger();
        public Trigger landTrigger = new Trigger();
        public Trigger doubleJumpTrigger = new Trigger();
        public Trigger fallTrigger = new Trigger();
        public Trigger rollTrigger = new Trigger();
        public Trigger attackTrigger = new Trigger();
        public Trigger staggerTrigger = new Trigger();

        // Mutable Variables
        public bool lockOn = false;
        public bool grounded = false;
        public DashDirection dashDirection;
        public StaggerDirection staggerDirection;

        // TEMP
        public Attack attackIndex;

        // Movement Variables
        public Vector3 currentVelocity = Vector3.zero;
        public Vector3 moveVector = Vector3.zero;
        public Vector3 targetDashDirection = Vector3.zero;

        // Jumping Variables
        public int jumpsUsed = 0;
        public int maxJumps = 2;
        public float fallingVelocity = 0;

        // In Air Variable
        public float maxAirVelocity = 2f;
        public float minAirVelocity = -2f; 

        public void ChangeState(State newState)
        {
            if (newState == currentState)
            {
                return;
            }

            lastState = currentState;
            currentState = newState;
        }
    }
}
