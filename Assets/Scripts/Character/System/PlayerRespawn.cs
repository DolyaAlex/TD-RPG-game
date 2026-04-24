using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Transform respawnPoint;

    [Header("Player Components")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private PlayerRotator playerRotator;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private CharacterController characterController;

    [Header("Visuals")]
    [SerializeField] private GameObject modelRoot;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 10f;

    private Collider[] cachedColliders;
    private bool isRespawning;

    private void Reset()
    {
        health = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
        playerMover = GetComponent<PlayerMover>();
        playerRotator = GetComponent<PlayerRotator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        cachedColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        if (isRespawning)
            return;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        SetPlayerActiveState(false);

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = respawnPoint != null
            ? respawnPoint.position
            : transform.position;

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = respawnPosition;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        health.RestoreFullHealth();
        SetPlayerActiveState(true);

        isRespawning = false;
    }

    private void SetPlayerActiveState(bool isActive)
    {
        if (inputReader != null)
            inputReader.enabled = isActive;

        if (playerMover != null)
            playerMover.enabled = isActive;

        if (playerRotator != null)
            playerRotator.enabled = isActive;

        if (playerAttack != null)
            playerAttack.enabled = isActive;

        if (characterController != null)
            characterController.enabled = isActive;

        SetVisualState(isActive);
        SetCollidersState(isActive);
    }

    private void SetVisualState(bool isVisible)
    {
        if (modelRoot != null)
        {
            modelRoot.SetActive(isVisible);
        }
    }

    private void SetCollidersState(bool isEnabled)
    {
        if (cachedColliders == null)
            return;

        foreach (Collider col in cachedColliders)
        {
            if (characterController != null && col == characterController)
                continue;

            col.enabled = isEnabled;
        }
    }
}
