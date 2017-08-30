namespace BenCo.Framework
{
    public class Timer
    {
        public int listIndex = -1;
        public float elapsedTime = 0f;
        public bool active;

        private float setTime;
        private bool restartTimerAfterCheck = false;

        public bool isTriggered
        {
            get
            {
                bool triggered = elapsedTime >= setTime;
                if (triggered)
                {
                    if (!restartTimerAfterCheck)
                    {
                        active = false;
                    }
                    elapsedTime = 0f;
                }
                return triggered;
            }
        }

        public Timer(int listIndex, float setTime, bool restartTimerAfterCheck = false)
        {
            this.listIndex = listIndex;
            this.setTime = setTime;
            this.restartTimerAfterCheck = restartTimerAfterCheck;
        }

        public void Reset()
        {
            elapsedTime = 0f;
        }
    }
}