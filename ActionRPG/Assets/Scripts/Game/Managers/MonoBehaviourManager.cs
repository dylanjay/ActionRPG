using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MonoBehaviourManager : MonoBehaviour
    {
        public static MonoBehaviourManager Instance { get; private set; }

        public enum UpdateType { Update, FixedUpdate, LateUpdate }

        public delegate void UpdateDelegate();
        private event UpdateDelegate updateEvent;
        private event UpdateDelegate fixedUpdateEvent;
        private event UpdateDelegate lateUpdateEvent;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (updateEvent != null)
            {
                updateEvent.Invoke();
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
    }
}
