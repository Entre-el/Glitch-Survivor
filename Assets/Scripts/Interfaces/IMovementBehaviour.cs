using UnityEngine;

public interface IMovementBehaviour
{
    void TickMovement(float deltaTime, ProjectileBase entity);
}