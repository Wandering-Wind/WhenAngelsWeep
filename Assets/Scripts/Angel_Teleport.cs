using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
public class Angel_Teleport : NetworkBehaviour
{
    [Header("Teleport")]
    [SerializeField] public Transform Angel_pos_curr;
    [SerializeField] public GameObject Angel_pos_01;
    [SerializeField] public GameObject Angel_pos_02;
    [SerializeField] public GameObject Angel_pos_03;
    private PlayerInput pi;
    private InputAction teleportAction;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        pi = GetComponent<PlayerInput>();
        teleportAction = pi.actions["Teleport"];
        teleportAction.Enable();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (teleportAction.WasPressedThisFrame())
        {
            TeleportServerRpc();
        }
    }

    [ServerRpc]
    private void TeleportServerRpc()
    {

    }
}
