using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : WorkableEntity
{
    public override string PrefabName { get { return "Zombie"; } }

    public override WorkManager WorkManager { get { return WorkManager.ZombieWorkManager; } }
}
