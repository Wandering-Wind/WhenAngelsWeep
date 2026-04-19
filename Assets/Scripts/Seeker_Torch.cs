using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Seeker_Torch : NetworkBehaviour
{
    [Header("Torch Settings")]
    public GameObject playerTorch;
    public GameObject torchLight;
    private PlayerInput pi;
    private InputAction torchAction;
    private bool isTorchOn = false;
    [SerializeField] private float raycastRange = 7f;
    public NetworkVariable<bool> isTorchOnNet = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            pi = GetComponent<PlayerInput>();
            torchAction = pi.actions["Torch"];
            torchAction.Enable();
        }
        pi = GetComponent<PlayerInput>();
        torchAction = pi.actions["Torch"];
        torchAction.Enable();
    }
    private void Update()
    {
        bool isTorchOn = isTorchOnNet.Value;

        if (torchLight && torchLight.activeSelf != isTorchOn)
        {
            torchLight.SetActive(isTorchOn);
        }

        if (!IsOwner) return;

        if (torchAction.WasPressedThisFrame())
        {
            isTorchOnNet.Value = true;
        }

        if (torchAction.WasReleasedThisFrame())
        {
            isTorchOnNet.Value = false;
        }

        if (isTorchOn)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerTorch.transform.position, playerTorch.transform.forward, out hit, raycastRange))
            {
                if (hit.collider.CompareTag("Angel"))
                {
                    print("Ahhh");
                }
            }

            Debug.DrawRay(playerTorch.transform.position, playerTorch.transform.forward * raycastRange, Color.red);
        }
    }
}



