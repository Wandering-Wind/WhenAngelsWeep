using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Seeker_Interact : NetworkBehaviour
{
    [Header("Interact Settings")]
    public Transform playerInteract;
    private PlayerInput pi;
    private InputAction interactAction;
    [SerializeField] private float InteractRange = 3f;
    [SerializeField] private Angel_Movment angelTarget;
    [SerializeField] private Camera interactCamera;

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
            Debug.Log("INTERACT PRESSED");
            TryInteractServerRpc();
        }
    }
    [ServerRpc]
    private void TryInteractServerRpc()
    {
        if (!interactCamera) return;

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * InteractRange, Color.green, 1f);

        if (Physics.Raycast(ray, out hit, InteractRange))
        {
            NetworkObject netObj = hit.collider.GetComponentInParent<NetworkObject>();

            if (netObj != null)
            {
                InteractServerRpc(netObj.NetworkObjectId);
            }
        }
    }
    [ServerRpc]
    public void InteractServerRpc(ulong objectId)
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
                if (angelTarget != null)
                {
                    //Debug.Log("NOO    Works");
                    angelTarget.FakeSpeedInc();
                }
            }

        }
    }
}

