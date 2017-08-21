using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Managers;

namespace BenCo.Framework
{
    public class MonoBehaviourWrapper : MonoBehaviour
    {
        private bool callUpdate = false;
        private bool callFixedUpdate = false;
        private bool callLateUpdate = false;

        protected virtual void MyUpdate() { }
        protected virtual void MyFixedUpdate() { }
        protected virtual void MyLateUpdate() { }

        public delegate void Task();

        public void Invoke(Task task, float time)
        {
            Invoke(task.Method.Name, time);
        }

        public void StartCoroutine(Task task, object value)
        {
            StartCoroutine(task.Method.Name, value);
        }

        protected void SetUpdateFlags(MonoBehaviourManager.UpdateDelegate myUpdate, MonoBehaviourManager.UpdateType updateType, bool active)
        {
            switch (updateType)
            {
                case MonoBehaviourManager.UpdateType.FixedUpdate:
                    if (callFixedUpdate != active)
                    {
                        MonoBehaviourManager.Instance.SetUpdate(myUpdate, MonoBehaviourManager.UpdateType.FixedUpdate, active);
                        callFixedUpdate = active;
                    }
                    break;
                case MonoBehaviourManager.UpdateType.LateUpdate:
                    if (callLateUpdate != active)
                    {
                        MonoBehaviourManager.Instance.SetUpdate(myUpdate, MonoBehaviourManager.UpdateType.LateUpdate, active);
                        callLateUpdate = active;
                    }
                    break;
                default:
                    if (callUpdate != active)
                    {
                        MonoBehaviourManager.Instance.SetUpdate(myUpdate, MonoBehaviourManager.UpdateType.Update, active);
                        callUpdate = active;
                    }
                    break;
            }
        }

        protected void InitializeUpdateFlags(bool callUpdate = false, bool callFixedUpdate = false, bool callLateUpdate = false)
        {
            if (callUpdate)
            {
                SetUpdateFlags(MyUpdate, MonoBehaviourManager.UpdateType.Update, true);
            }
            if (callFixedUpdate)
            {
                SetUpdateFlags(MyFixedUpdate, MonoBehaviourManager.UpdateType.FixedUpdate, true);
            }
            if (callLateUpdate)
            {
                SetUpdateFlags(MyLateUpdate, MonoBehaviourManager.UpdateType.LateUpdate, true);
            }
        }
    }
}
