using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableEntity : Entity, ISeeingEntity {

    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private float _seeingRange = 5;

    public float SightRange { get { return _seeingRange; } }

    private Vector2? targetPosition;

    public void SetTargetPosition(Vector2 position)
    {
        if (targetPosition == position)
            return;

        targetPosition = position;
    }
    protected virtual void Update()
    {
        if(targetPosition.HasValue)
            Move();
    }
    private void Move()
    {
        Vector2 moveDelta = targetPosition.Value - (Vector2)transform.position;
        float moveDistance = (_speed * Time.deltaTime);

        if(moveDelta.magnitude < moveDistance)
        {
            transform.position = targetPosition.Value;

            targetPosition = null;
        }
        else
        {
            transform.position += (Vector3)moveDelta.normalized * moveDistance;
        }        
    }
}
