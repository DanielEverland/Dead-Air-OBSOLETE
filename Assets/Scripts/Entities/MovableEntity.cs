using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableEntity : SeeingEntity {

    [SerializeField]
    private float _speed = 1;

    private Vector2? _targetPosition;
        
    public void SetTargetPosition(Vector2 position)
    {
        if (_targetPosition == position)
            return;

        _targetPosition = position;
    }
    protected virtual void Update()
    {
        if(_targetPosition.HasValue)
            Move();
    }
    private void Move()
    {
        Vector2 moveDelta = _targetPosition.Value - (Vector2)transform.position;
        float moveDistance = (_speed * Time.deltaTime);

        if(moveDelta.magnitude < moveDistance)
        {
            transform.position = _targetPosition.Value;

            _targetPosition = null;
        }
        else
        {
            transform.position += (Vector3)moveDelta.normalized * moveDistance;
        }

        PollRegion();
    }
}
