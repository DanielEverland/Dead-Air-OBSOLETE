using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWork {
    
    void Update(WorkableEntity caller);
    bool IsDone(WorkableEntity caller);
}
