using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SeeingEntity : VisibleEntity, ISeeingEntity {
    
    [SerializeField]
    private float _seeingRange = 5;

    public float SightRange { get { return _seeingRange; } }

    protected override void Awake()
    {
        base.Awake();

        VisionManager.AddVisibleEntity(this);
    }
    public void SightEnter(Entity entity)
    {
        OnSightEnter(entity);
    }
    public void SightLeave(Entity entity)
    {
        OnSightLeave(entity);
    }
    public void SightStay(Entity entity)
    {
        OnSightStay(entity);
    }
    protected virtual void OnSightEnter(Entity entity) { }
    protected virtual void OnSightStay(Entity entity) { }
    protected virtual void OnSightLeave(Entity entity) { }
    protected override void OnDead()
    {
        base.OnDead();

        VisionManager.RemoveVisibleEntity(this);
    }
}
