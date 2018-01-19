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
    public void OnSightEnter(Entity entity)
    {
        
    }
    public void OnSightLeave(Entity entity)
    {
        
    }
    public void OnSightStay(Entity entity)
    {
        
    }
    protected override void OnDead()
    {
        base.OnDead();

        VisionManager.RemoveVisibleEntity(this);
    }
}
