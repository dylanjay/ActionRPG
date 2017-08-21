using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Framework;
using BenCo.Extensions;

namespace BenCo.Player
{
    public class PlayerView : MonoBehaviourWrapper
    {
        // Model for MVC
        private PlayerModel model;

        private Animator animator;

        private static class AnimParams
        {
            public static readonly int velocityX = Animator.StringToHash("VelocityX");
            public static readonly int velocityZ = Animator.StringToHash("VelocityZ");
            public static readonly int moving = Animator.StringToHash("Moving");
            public static readonly int stagger = Animator.StringToHash("Stagger");
            public static readonly int staggerDirection = Animator.StringToHash("StaggerDirection");
            public static readonly int die = Animator.StringToHash("Die");
            public static readonly int revive = Animator.StringToHash("Revive");
            public static readonly int jumpIndex = Animator.StringToHash("JumpIndex");
            public static readonly int jump = Animator.StringToHash("Jump");
            public static readonly int roll = Animator.StringToHash("Roll");
            public static readonly int rollDirection = Animator.StringToHash("RollDirection");
            public static readonly int attack = Animator.StringToHash("Attack");
            public static readonly int attackIndex = Animator.StringToHash("AttackIndex");
            public static readonly int lockOn = Animator.StringToHash("LockOn");
        }

        private void Start()
        {
            InitializeUpdateFlags(false, false, true);

            model = GetComponent<PlayerModel>();
            if (!model)
            {
                Debug.LogErrorFormat("{0} has no Player Model", this);
            }

            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Player has no Animator", this);
            }
            animator.applyRootMotion = false;
        }

        protected override void MyLateUpdate()
        {
            UpdateAnimator();
        }

        void UpdateAnimator()
        {
            // set root motion
            if (model.rootMotionTrigger.isTriggered)
            {
                animator.applyRootMotion = model.rootMotionTrigger.value;
            }

            //Get local velocity of charcter
            float xVelocity = transform.InverseTransformDirection(model.currentVelocity).x;
            float yVelocity = transform.InverseTransformDirection(model.currentVelocity).z;
            //Update animator with movement values
            animator.SetFloat(AnimParams.velocityX, xVelocity / model.moveSpeed);
            animator.SetFloat(AnimParams.velocityZ, yVelocity / model.moveSpeed);

            if (model.lockOnTrigger.isTriggered)
            {
                animator.SetBool(AnimParams.lockOn, model.lockOn);
            }

            //if character is alive and can move, set our animator
            if (!model.isDead && model.canMove)
            {
                if (model.currentVelocity.magnitude > 0.00001f)
                {
                    animator.SetBool(AnimParams.moving, true);
                }
                else
                {
                    animator.SetBool(AnimParams.moving, false);
                }
            }

            if (model.staggerTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.staggerDirection, (int)model.staggerDirection);
                animator.SetTrigger(AnimParams.stagger);
            }

            if (model.dieTrigger.isTriggered)
            {
                animator.SetTrigger(AnimParams.die);
            }

            if (model.reviveTrigger.isTriggered)
            {
                animator.SetTrigger(AnimParams.revive);
            }

            if (model.landTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.jumpIndex, 0);
            }

            if (model.jumpTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.jumpIndex, 1);
                animator.SetTrigger(AnimParams.jump);
            }

            if (model.fallTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.jumpIndex, 2);
                animator.SetTrigger(AnimParams.jump);
            }

            if (model.rollTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.rollDirection, (int)model.dashDirection);
                animator.SetTrigger(AnimParams.roll);
            }

            if (model.attackTrigger.isTriggered)
            {
                animator.SetInteger(AnimParams.attackIndex, (int)model.attackIndex);
                animator.SetTrigger(AnimParams.attack);
            }
        }
    }
}