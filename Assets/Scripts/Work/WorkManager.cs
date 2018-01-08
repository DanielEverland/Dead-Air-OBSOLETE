using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkManager {

    public static WorkManager PlayerWorkManager { get { return _playerWorkManager; } }
    public static WorkManager ZombieWorkManager { get { return _zombieWorkManager; } }

    private static WorkManager _playerWorkManager = new WorkManager();
    private static WorkManager _zombieWorkManager = new WorkManager();

    public bool HasWork { get { return QueuedWork.Count > 0; } }

    private Queue<IWork> QueuedWork = new Queue<IWork>();

    public void QueueWork(IWork work)
    {
        QueuedWork.Enqueue(work);
    }
    public IWork GetWork()
    {
        if (QueuedWork.Count == 0)
            return null;

        return QueuedWork.Dequeue();
    }
}
