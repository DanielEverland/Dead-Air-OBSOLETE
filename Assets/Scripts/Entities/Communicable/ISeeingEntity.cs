using UnityEngine;

public interface ISeeingEntity {

	float SightRange { get; }
    Vector2 Position { get; }

    void OnSightEnter(Entity entity);
    void OnSightStay(Entity entity);
    void OnSightLeave(Entity entity);
}
