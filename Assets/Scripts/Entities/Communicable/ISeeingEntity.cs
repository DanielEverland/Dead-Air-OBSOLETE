using UnityEngine;

public interface ISeeingEntity {

	float SightRange { get; }
    Vector2 Position { get; }

    void SightEnter(Entity entity);
    void SightStay(Entity entity);
    void SightLeave(Entity entity);
}
