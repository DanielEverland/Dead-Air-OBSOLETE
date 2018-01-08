using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HordeManager {

    /// <summary>
    /// Interval in seconds between each tick
    /// </summary>
    private const float TICK_INTERVAL = 5;

    /// <summary>
    /// Max distance between zombies to form a horde
    /// </summary>
    private const float MAX_DISTANCE = 8;

    /// <summary>
    /// How many zombies does it take to create a horde?
    /// </summary>
    private const int HORDE_MIN_COUNT = 5;

    private static float lastTickTime;
    private static List<Horde> hordes;

    private static readonly Color DRAW_COLOR = Color.yellow;

    public static void Initialize()
    {
        Tick();
    }
    public static void Update()
    {
        CheckForTick();
        hordes.ForEach(x => x.Update());
    }
    private static void CheckForTick()
    {
        if(Time.unscaledTime - lastTickTime >= TICK_INTERVAL)
        {
            Tick();
        }
    }
    public static void Draw()
    {
        for (int i = 0; i < hordes.Count; i++)
        {
            Debug.Log(hordes[i].Rect);

            EG_Debug.DrawRect(hordes[i].Rect, DRAW_COLOR);
        }
    }
    private static void Tick()
    {
        lastTickTime = Time.unscaledTime;

        List<Zombie> allZombies = new List<Zombie>(Game.GetEntitiesOfType<Zombie>());
        hordes = new List<Horde>();

        while (allZombies.Count > 0)
        {
            Horde horde = new Horde();
            horde.Clear();

            PollNearbyZombies(allZombies[0], horde, allZombies);

            if(horde.Count >= HORDE_MIN_COUNT)
                hordes.Add(horde);
        }

        for (int i = 0; i < hordes.Count; i++)
        {
            hordes[i].Intialize();
        }
    }
    private static void PollNearbyZombies(Zombie zombie, Horde horde, List<Zombie> collection)
    {
        collection.Remove(zombie);
        horde.Add(zombie);

        if (collection.Count == 0)
            return;

        Zombie nearestZombie = GetNearest(zombie.transform.position, collection);

        if (Vector2.Distance(nearestZombie.transform.position, zombie.transform.position) < MAX_DISTANCE)   
        {
            PollNearbyZombies(nearestZombie, horde, collection);
        }
    }
    private static Zombie GetNearest(Vector2 position, List<Zombie> collection)
    {
        float min = float.MaxValue;
        float currentDistance;
        Zombie obj = null;
        Zombie currentZombie;

        for (int i = 0; i < collection.Count; i++)
        {
            currentZombie = collection[i];
            currentDistance = Vector2.Distance(currentZombie.transform.position, position);

            if (currentDistance < min)
            {
                min = currentDistance;
                obj = currentZombie;
            }
        }

        return obj;
    }
}
