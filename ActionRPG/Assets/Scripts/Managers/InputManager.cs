using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour 
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

    private static InputManager _input;
    public static InputManager input
    {
        get
        {
            if (!_input)
            {
                Debug.LogError("No instance of InputManager in scene");
            }
            return _input;
        }
    }

    public Axis2D move = new Axis2D("Horizontal", "Vertical");
    public Button jump = new Button("Jump");
    public Button attack = new Button("Fire1");
    public Button sprint = new Button("Sprint");

    private void Awake()
    {
        _input = this;
    }
}
