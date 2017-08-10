using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Framework;

namespace Managers
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
        public static Button attack = new Button("Fire1");
        public static Button sprint = new Button("Sprint");

        private void Awake()
        {
            Instance = this;
        }
    }
}
