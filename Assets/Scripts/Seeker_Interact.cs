using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Seeker_Interact : NetworkBehaviour
{
    [Header("Interact Settings")]
    public Transform playerInteract;
    [SerializeField] private float InteractRange = 3f;
    [SerializeField] private Angel_Movment angelTarget;
    [SerializeField] private Camera interactCamera;
    [SerializeField] private GameObject WinScreenSeeker;

    [Header("Animation")]
    [SerializeField] private string isinteract = "DoInteract";
    [SerializeField] private Animator animator;

    private PlayerInput pi;
    private InputAction interactAction;

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
            TryInteract();
            if (animator)
                animator.SetTrigger(isinteract);
        }
    }

    private void TryInteract()
    {
        if (interactCamera == null) return;

        Ray ray = interactCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * InteractRange, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractRange))
        {
            Debug.Log("HIT: " + hit.collider.name);
        
            NetworkObject netObj = hit.collider.GetComponentInParent<NetworkObject>();

            if (netObj == null)
            {
                Debug.LogError("NO NetworkObject on hit!");
                return;
            }
            InteractServerRpc(netObj.NetworkObjectId);
        }
        else
        {
            Debug.Log("NO HIT");
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void InteractServerRpc(ulong objectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            if (netObj.CompareTag("Artifact"))
            {
                Debug.Log("REAL");
                WinScreenSeeker.SetActive(true);
                Time.timeScale = 0f;
            }
            else if (netObj.CompareTag("Fake_Artifact"))
            {
                Debug.Log("NOOOOOOOOO");
                if (angelTarget != null)
                {
                    angelTarget.FakeSpeedInc();
                }
            }
        }
    }
}
