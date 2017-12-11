using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWork {

    void SetOwner(WorkableEntity owner);
    void Poll();
    bool IsDone();
}
