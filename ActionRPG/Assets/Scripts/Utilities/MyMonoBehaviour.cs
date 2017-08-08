using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMonoBehaviour : MonoBehaviour
{
    private bool callUpdate = false;
    private bool callFixedUpdate = false;
    private bool callLateUpdate = false;

    protected virtual void MyUpdate() { }
    protected virtual void MyFixedUpdate() { }
    protected virtual void MyLateUpdate() { }

    protected void SetUpdateFlags(MonoBehaviourManager.MyUpdate myUpdate, MonoBehaviourManager.UpdateType updateType, bool active)
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

    //protected void SetUpdateFlags(MonobehaviourManager.UpdateArray.MyUpdate myUpdate, MonobehaviourManager.UpdateType updateType, bool active)
    //{
    //    switch (updateType)
    //    {
    //        case MonobehaviourManager.UpdateType.FixedUpdate:
    //            if (callFixedUpdate != active)
    //            {
    //                MonobehaviourManager.Instance.SetUpdate(myUpdate, MonobehaviourManager.UpdateType.FixedUpdate, active);
    //                callFixedUpdate = active;
    //            }
    //            break;
    //        case MonobehaviourManager.UpdateType.LateUpdate:
    //            if (callLateUpdate != active)
    //            {
    //                MonobehaviourManager.Instance.SetUpdate(myUpdate, MonobehaviourManager.UpdateType.LateUpdate, active);
    //                callLateUpdate = active;
    //            }
    //            break;
    //        default:
    //            if (callUpdate != active)
    //            {
    //                MonobehaviourManager.Instance.SetUpdate(myUpdate, MonobehaviourManager.UpdateType.Update, active);
    //                callUpdate = active;
    //            }
    //            break;
    //    }
    //}

    //protected void InitializeUpdateFlags(MonobehaviourManager.UpdateArray.MyUpdate update = null, MonobehaviourManager.UpdateArray.MyUpdate fixedUpdate = null, MonobehaviourManager.UpdateArray.MyUpdate lateUpdate = null)
    //{
    //    if (update != null)
    //    {
    //        SetUpdateFlags(update, MonobehaviourManager.UpdateType.Update, true);
    //    }
    //    if (fixedUpdate != null)
    //    {
    //        SetUpdateFlags(fixedUpdate, MonobehaviourManager.UpdateType.FixedUpdate, true);
    //    }
    //    if (lateUpdate != null)
    //    {
    //        SetUpdateFlags(lateUpdate, MonobehaviourManager.UpdateType.LateUpdate, true);
    //    }
    //}
}
