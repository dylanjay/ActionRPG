/// <summary>
/// Model base class used for the MVC framework
/// </summary>

namespace BenCo.Framework
{
	public class Model : MonoBehaviourWrapper 
	{
		public class Trigger
        {
            private bool trigger = false;
            public bool isTriggered
            {
                get
                {
                    if (trigger)
                    {
                        trigger = false;
                        return true;
                    }
                    return false;
                }
            }

            public void Set()
            {
                trigger = true;
            }
        }

        public class DirtyTrigger
        {
            private bool trigger = false;
            public bool value;
            public bool isTriggered
            {
                get
                {
                    if (trigger)
                    {
                        trigger = false;
                        return true;
                    }
                    return false;
                }
            }

            public void Set(bool value)
            {
                this.value = value;
                trigger = true;
            }
        }
	}
}