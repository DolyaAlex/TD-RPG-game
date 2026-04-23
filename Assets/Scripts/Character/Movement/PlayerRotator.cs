using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float rotationSpeed = 15f;

    private void Update()
    {
        if (playerAttack != null && playerAttack.IsAttackingThisFrame)
            return;

        Vector3 moveDirection = playerMover.MoveDirection;

        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
