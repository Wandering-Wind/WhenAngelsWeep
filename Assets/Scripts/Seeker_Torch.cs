using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Seeker_Torch : NetworkBehaviour
{
    [Header("Torch Settings")]
    public GameObject playerTorch;
    public GameObject torchLight;
    public float raycastDitance = 7f;
    private PlayerInput pi;
    private InputAction torchAction;
    private NetworkVariable<bool> isTorchOn = new NetworkVariable<bool>();


    public override void OnNetworkSpawn()
    {
        if (torchLight)
        {
            isTorchOn.OnValueChanged += OnTorchChanged;
            OnTorchChanged(false, isTorchOn.Value);
        }

        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        pi = GetComponent<PlayerInput>();
        torchAction = pi.actions["Torch"];
        torchAction.Enable();
    }
    private void OnTorchChanged(bool oldValue, bool newValue)
    {
        torchLight.SetActive(newValue);
    }
    private void Update()
    {

        if (!IsOwner) return;
        if (torchAction.WasPressedThisFrame())
            OnTorchServerRpc();

        if (torchAction.WasReleasedThisFrame())
            OffTorchServerRpc();

        if (isTorchOn.Value)
        {
            RaycastTorchServerRpc();
        }
    }
    [ServerRpc]
    private void OnTorchServerRpc()
    {
        isTorchOn.Value = true;
    }
    [ServerRpc]
    private void OffTorchServerRpc() 
    { 
        isTorchOn.Value = false;  
    }
    [ServerRpc]
    private void RaycastTorchServerRpc()
    {
        Vector3 origin = playerTorch.transform.position;
        Vector3 dir = playerTorch.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, raycastDitance))
        {
            Debug.DrawRay(origin, dir * raycastDitance, Color.red);

            if (hit.collider.CompareTag("Angel"))
            {
                var netObj = hit.collider.GetComponent<NetworkObject>();

                if (netObj != null)
                {
                    FreezeAngelServerRpc(netObj.NetworkObjectId);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FreezeAngelServerRpc(ulong angelId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(angelId, out var obj))
        {
            var angel = obj.GetComponent<Angel_Movment>();

            if (angel != null)
            {
                angel.FreezeServerRpc();
            }
        }
    }
}



