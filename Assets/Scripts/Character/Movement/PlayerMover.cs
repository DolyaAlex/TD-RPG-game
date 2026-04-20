using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedGravity = -2f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    public Vector3 MoveDirection { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = inputReader.MoveInput;
        Vector3 inputDirection = new Vector3(input.x, 0f, input.y).normalized;

        MoveDirection = inputDirection;

        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedGravity;
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 movement = inputDirection * moveSpeed;
        Vector3 finalMove = movement + verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }
}