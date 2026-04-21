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
        RaycastHit hit;
        if (Physics.Raycast(playerTorch.transform.position, playerTorch.transform.forward, out hit, raycastDitance))
        {
            if (hit.collider.CompareTag("Angel"))
            {
                print("Ahhh");
                var angel = hit.collider.GetComponent<Angel_Movment>();
                if (angel != null)
                {
                    angel.FreezeServerRpc();
                }
            }
        }
        Debug.DrawRay(playerTorch.transform.position, playerTorch.transform.forward * raycastDitance, Color.red);
    }
}



