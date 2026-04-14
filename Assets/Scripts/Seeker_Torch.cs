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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false; 
            return;
        }
        pi = GetComponent<PlayerInput>();
        torchAction = pi.actions["Torch"];
        torchAction.Enable(); 
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (torchAction.WasPressedThisFrame())
            OnTorchServerRpc();
        if (torchAction.WasReleasedThisFrame())
        {
            isTorchOn = false;
            torchLight.SetActive(false);
        }

        if (isTorchOn)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerTorch.transform.position, playerTorch.transform.forward, out hit, 20f))
            {
                if (hit.collider.CompareTag("Angel"))
                {
                    print("Ahhh");
                }
            }
            Debug.DrawRay(playerTorch.transform.position, playerTorch.transform.forward * 20f, Color.red);
        }
    }

    [ServerRpc]
    private void OnTorchServerRpc()
    {
        isTorchOn = true;
        if (torchLight)
        {
            torchLight.SetActive(isTorchOn);
        }
    }
}



