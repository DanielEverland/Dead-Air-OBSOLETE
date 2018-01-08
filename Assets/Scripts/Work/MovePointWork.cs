using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePointWork : Work
{
    public MovePointWork(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private readonly Vector2 targetPosition;

    protected override void Update()
    {
        Owner.SetTargetPosition(targetPosition);
    }
    public override bool IsDone()
    {
        return (Vector2)Owner.transform.position == targetPosition;
    }
    public static Vector2 GetRandomPosition(Vector2 anchor, float radius)
    {
        Vector2 direction = new Vector2()
        {
            x = Random.Range(-10, 10),
            y = Random.Range(-10, 10),
        };
        
        Vector2 randomPos = anchor + direction.normalized * Random.Range(1, radius);

        randomPos.x = Mathf.RoundToInt(randomPos.x);
        randomPos.y = Mathf.RoundToInt(randomPos.y);

        return randomPos;
    }
}
