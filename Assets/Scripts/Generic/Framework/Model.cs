/// <summary>
/// Model base class used for the MVC framework
/// </summary>
using System.Collections;
using System.Collections.Generic;

namespace BenCo.Framework
{
	public class Model : MonoBehaviourWrapper 
	{
		public class Trigger
        {
            private bool trigger = false;
            private bool _value;
            public bool isTriggered
            {
                get
                {
                    return trigger;
                }
            }

            public bool value
            {
                get
                {
                    return _value;
                }
            }

            public IEnumerator Set()
            {
                trigger = true;
                yield return null;
                trigger = false;
            }

            public IEnumerator Set(bool value)
            {
                trigger = true;
                _value = value;
                yield return null;
                trigger = false;
            }
        }
	}
}