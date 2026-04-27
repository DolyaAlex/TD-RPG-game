using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private PlayerMover mover;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float rotationSpeed = 10f;

    private void Update()
    {
        Vector3 targetDirection = Vector3.zero;

        if (playerAttack != null && playerAttack.IsAttackingThisFrame)
        {
            targetDirection = playerAttack.AimDirection;
        }
        else if (mover != null && mover.MoveDirection.sqrMagnitude > 0.001f)
        {
            targetDirection = mover.MoveDirection.normalized;
        }

        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}