using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Angel_Teleport : NetworkBehaviour
{
    private PlayerInput pi;
    private InputAction teleportAction;

    private Angel_Start_Place placementScript;

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

        placementScript = GetComponent<Angel_Start_Place>();
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
        if (placementScript == null) return;

        var positions = placementScript.placedPositions;

        if (positions == null || positions.Count == 0)
            return;

        int index = Random.Range(0, positions.Count);
        Vector3 targetPos = positions[index];

        CharacterController cc = GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            transform.position = targetPos;
            cc.enabled = true;
        }
        else
        {
            transform.position = targetPos;
        }
    }
}