using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Framework;

namespace BenCo.Managers
{
    public class MonoBehaviourManager : MonoBehaviour
    {
        public static MonoBehaviourManager Instance { get; private set; }

        public enum UpdateType { Update, FixedUpdate, LateUpdate }

        public delegate void UpdateDelegate();
        private event UpdateDelegate updateEvent;
        private event UpdateDelegate fixedUpdateEvent;
        private event UpdateDelegate lateUpdateEvent;

        private List<Timer> timers = new List<Timer>();

        public float deltaTime;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // Set cached delta time variable for all monobehaviours to use
            deltaTime = Time.deltaTime;

            if (updateEvent != null)
            {
                updateEvent.Invoke();
            }

            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].active)
                {
                    timers[i].elapsedTime += deltaTime;
                }
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdateEvent != null)
            {
                fixedUpdateEvent.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (lateUpdateEvent != null)
            {
                lateUpdateEvent.Invoke();
            }
        }

        public void AddUpdate(UpdateDelegate myUpdate, UpdateType type)
        {
            switch (type)
            {
                case UpdateType.Update:
                    updateEvent += myUpdate;
                    break;
                case UpdateType.FixedUpdate:
                    fixedUpdateEvent += myUpdate;
                    break;
                case UpdateType.LateUpdate:
                    lateUpdateEvent += myUpdate;
                    break;
            }
        }

        public void RemoveUpdate(UpdateDelegate myUpdate, UpdateType type)
        {
            switch (type)
            {
                case UpdateType.Update:
                    updateEvent -= myUpdate;
                    break;
                case UpdateType.FixedUpdate:
                    fixedUpdateEvent -= myUpdate;
                    break;
                case UpdateType.LateUpdate:
                    lateUpdateEvent -= myUpdate;
                    break;
            }
        }

        public void SetUpdate(UpdateDelegate myUpdate, UpdateType type, bool subscribe)
        {
            switch (type)
            {
                case UpdateType.Update:
                    updateEvent = subscribe ? updateEvent + myUpdate : updateEvent - myUpdate;
                    break;
                case UpdateType.FixedUpdate:
                    fixedUpdateEvent = subscribe ? fixedUpdateEvent + myUpdate : fixedUpdateEvent - myUpdate;
                    break;
                case UpdateType.LateUpdate:
                    lateUpdateEvent = subscribe ? lateUpdateEvent + myUpdate : lateUpdateEvent - myUpdate;
                    break;
            }
        }

        public Timer AddTimer(float setTime, bool restartTimerAfterCheck = false)
        {
            Timer timer = new Timer(timers.Count - 1, setTime, restartTimerAfterCheck);
            timers.Add(timer);
            return timer;
        }
    }
}
