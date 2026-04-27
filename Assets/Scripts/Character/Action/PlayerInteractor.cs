using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayers;

    private void Update()
    {
        if (inputReader == null)
            return;

        if (!inputReader.InteractPressedThisFrame)
            return;

        TryInteract();
    }

    private void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRadius,
            interactableLayers,
            QueryTriggerInteraction.Collide
        );

        IInteractable closestInteractable = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            Vector3 closestPoint = hit.ClosestPoint(transform.position);
            float distanceSqr = (closestPoint - transform.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null)
        {
            Debug.Log("Interact executed");
            closestInteractable.Interact(gameObject);
        }
        else
        {
            Debug.Log("No interactable object nearby.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}