using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private PlayerMover mover;
    [SerializeField] private float rotationSpeed = 12f;

    private void Update()
    {
        Vector3 direction = mover.MoveDirection;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
