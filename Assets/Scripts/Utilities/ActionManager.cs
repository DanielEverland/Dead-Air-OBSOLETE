using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    private static ActionManager _instance;

    private LinkedList<Callback> _callbacks = new LinkedList<Callback>();

    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
        LinkedListNode<Callback> node = _callbacks.First;

        while (node != null)
        {
            LinkedListNode<Callback> next = node.Next;

            node.Value.Poll();

            node = next;
        }
    }
    public static void DelayedCallback(float delay, System.Action callback)
    {
        _instance._callbacks.AddLast(new Callback(delay, callback));
    }
    public static void DelayedCallback<T>(float delay, System.Action callback, System.Func<T, bool> condition, T conditionObject)
    {
        _instance._callbacks.AddLast(new ConditionedCallback<T>(delay, callback, condition, conditionObject));
    }
    private static void RaiseCallback(Callback callback)
    {
        _instance._callbacks.Remove(callback);

        callback.Raise();
    }

    public class ConditionedCallback<T> : Callback
    {
        public ConditionedCallback(float delay, System.Action callback, System.Func<T, bool> condition, T obj) : base(delay, callback)
        {
            this.obj = obj;
            this.condition = condition;
        }

        private readonly T obj;
        private readonly System.Func<T, bool> condition;

        public override void Raise()
        {
            if (condition.Invoke(obj))
            {
                callback.Invoke();
            }
        }
    }
    public class Callback
    {
        public Callback(float delay, System.Action callback)
        {
            remainingTime = delay;
            
            this.callback = callback;
        }
        
        protected readonly System.Action callback;

        private float remainingTime;

        public void Poll()
        {
            remainingTime -= Time.unscaledDeltaTime;

            if(remainingTime <= 0)
            {
                RaiseCallback(this);
            }
        }
        public virtual void Raise()
        {
            callback.Invoke();
        }
    }
}
