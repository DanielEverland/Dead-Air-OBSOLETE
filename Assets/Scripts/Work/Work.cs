using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Work : IWork {
    
    public virtual void Update(WorkableEntity caller) { }

    public abstract bool IsDone(WorkableEntity caller);
}
