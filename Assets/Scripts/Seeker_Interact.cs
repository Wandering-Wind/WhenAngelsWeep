using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Seeker_Interact : NetworkBehaviour
{
    [Header("Torch Settings")]
    public Transform playerInteract;
    private PlayerInput pi;
    private InputAction interactAction;
    private bool isTorchOn = false;
    [SerializeField] private float InteractRange = 3f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        pi = GetComponent<PlayerInput>();
        interactAction = pi.actions["Interact"];
        interactAction.Enable();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (interactAction.WasPressedThisFrame())
        {
            Interact();
        }
    }
    public void Interact()
    {
        Ray ray = new Ray(playerInteract.position, playerInteract.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, InteractRange))
        {
            NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();

            if (netObj != null)
            {
                InteractServerRpc(netObj.NetworkObjectId);
            }
        }
    }
    [ServerRpc]
    private void InteractServerRpc(ulong objectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            if (netObj.CompareTag("Artifact"))
            {
                Debug.Log("REAL");
            }
            else if (netObj.CompareTag("Fake_Artifact"))
            {
                Debug.Log("NOOOOOOOOO");
            }
        }
    }

}

