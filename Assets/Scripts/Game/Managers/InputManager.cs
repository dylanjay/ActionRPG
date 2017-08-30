using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Extensions;
using BenCo.Framework;

namespace BenCo.Managers
{
    public class InputManager : Singleton<MonoBehaviourWrapper>
    {
        public class Button
        {
            private string buttonName;

            public Button(string axisName) { this.buttonName = axisName; }

            public bool isPressed
            {
                get
                {
                    return Input.GetButton(buttonName);
                }
            }

            public bool wasPressed
            {
                get
                {
                    return Input.GetButtonDown(buttonName);
                }
            }

            public bool wasReleased
            {
                get
                {
                    return Input.GetButtonUp(buttonName);
                }
            }
        }

        public class Axis
        {
            private string axisName;

            public Axis(string axisName) { this.axisName = axisName; }

            public float value
            {
                get
                {
                    return Input.GetAxis(axisName);
                }
            }

            public float valueRaw
            {
                get
                {
                    return Input.GetAxisRaw(axisName);
                }
            }
        }

        public class Axis2D
        {
            public Axis x;
            public Axis y;

            public Axis2D(string xAxisName, string yAxisName)
            {
                x = new Axis(xAxisName);
                y = new Axis(yAxisName);
            }
        }

        public static Axis2D move = new Axis2D("Horizontal", "Vertical");
        public static Button jump = new Button("Jump");
        public static Button attack = new Button("Attack");
        //public static Button leftAttack = new Button("LeftAttack");
        //public static Button rightAttack = new Button("RightAttack");
        //public static Button attackModifier = new Button("AttackModifier");
        public static Button stagger = new Button("Stagger");
        public static Button lockOn = new Button("LockOn");
        public static Button dash = new Button("Dash");
        public static Button die = new Button("Die");

        private void Awake()
        {
            Instance = this;
        }
    }
}
