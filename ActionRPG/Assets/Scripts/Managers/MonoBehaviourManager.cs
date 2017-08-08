using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourManager : MonoBehaviour
{
    public static MonoBehaviourManager Instance { get; private set; }

    public enum UpdateType { Update, FixedUpdate, LateUpdate }

    public delegate void MyUpdate();
    private event MyUpdate updateEvent;
    private event MyUpdate fixedUpdateEvent;
    private event MyUpdate lateUpdateEvent;

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

    public void AddUpdate(MyUpdate myUpdate, UpdateType type)
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

    public void RemoveUpdate(MyUpdate myUpdate, UpdateType type)
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

    public void SetUpdate(MyUpdate myUpdate, UpdateType type, bool subscribe)
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

    //public enum UpdateType { Update, FixedUpdate, LateUpdate }

    //public class UpdateArray
    //{
    //    // Delegate for this array
    //    public delegate void MyUpdate();
    //    // Array of delegates
    //    private MyUpdate[] myUpdates = new MyUpdate[1];
    //    // Current index of 
    //    private int updateIndex = 0;

    //    // Overload accessor operator
    //    internal MyUpdate this[int index]
    //    {
    //        get
    //        {
    //            return myUpdates[index];
    //        }
    //        set
    //        {
    //            myUpdates[index] = value;
    //        }
    //    }

    //    public int size
    //    {
    //        get
    //        {
    //            return updateIndex;
    //        }
    //    }

    //    public bool empty
    //    {
    //        get
    //        {
    //            return updateIndex == 0;
    //        }
    //    }

    //    public void Add(MyUpdate newUpdate)
    //    {
    //        // If update array is full double the size
    //        if (updateIndex == myUpdates.Length)
    //        {
    //            Array.Resize(ref myUpdates, updateIndex * 2);
    //        }

    //        // Push new update onto array and increment index
    //        myUpdates[updateIndex] = newUpdate;
    //        updateIndex++;
    //    }

    //    public void Remove(MyUpdate updateToRemove)
    //    {
    //        for (int i = 0; i < updateIndex; i++)
    //        {
    //            if (myUpdates[i] == updateToRemove)
    //            {
    //                myUpdates[i] = null;
    //                return;
    //            }
    //        }
    //    }

    //    public void Set(MyUpdate myUpdate, bool active)
    //    {
    //        if (active)
    //        {
    //            Add(myUpdate);
    //        }
    //        else
    //        {
    //            Remove(myUpdate);
    //        }
    //    }
    //}

    //private UpdateArray updateArray = new UpdateArray();
    //private UpdateArray fixedUpdateArray = new UpdateArray();
    //private UpdateArray lateUpdateArray = new UpdateArray();

    //private void Awake()
    //{
    //    Instance = this;
    //}

    //private void Update()
    //{
    //    for (int i = 0; i < updateArray.size; i++)
    //    {
    //        updateArray[i]();
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    for (int i = 0; i < fixedUpdateArray.size; i++)
    //    {
    //        fixedUpdateArray[i]();
    //    }
    //}

    //private void LateUpdate()
    //{
    //    for (int i = 0; i < lateUpdateArray.size; i++)
    //    {
    //        lateUpdateArray[i]();
    //    }
    //}

    //public void AddUpdate(UpdateArray.MyUpdate newUpdate, UpdateType type)
    //{
    //    switch (type)
    //    {
    //        case UpdateType.FixedUpdate:
    //            fixedUpdateArray.Add(newUpdate);
    //            break;
    //        case UpdateType.LateUpdate:
    //            lateUpdateArray.Add(newUpdate);
    //            break;
    //        default:
    //            updateArray.Add(newUpdate);
    //            break;
    //    }
    //}

    //public void RemoveUpdate(UpdateArray.MyUpdate newUpdate, UpdateType type)
    //{
    //    switch (type)
    //    {
    //        case UpdateType.FixedUpdate:
    //            fixedUpdateArray.Remove(newUpdate);
    //            break;
    //        case UpdateType.LateUpdate:
    //            lateUpdateArray.Remove(newUpdate);
    //            break;
    //        default:
    //            updateArray.Remove(newUpdate);
    //            break;
    //    }
    //}

    //public void SetUpdate(UpdateArray.MyUpdate myUpdate, UpdateType type, bool active)
    //{
    //    switch (type)
    //    {
    //        case UpdateType.FixedUpdate:
    //            fixedUpdateArray.Set(myUpdate, active);
    //            break;
    //        case UpdateType.LateUpdate:
    //            lateUpdateArray.Set(myUpdate, active);
    //            break;
    //        default:
    //            updateArray.Set(myUpdate, active);
    //            break;
    //    }
    //}
}
