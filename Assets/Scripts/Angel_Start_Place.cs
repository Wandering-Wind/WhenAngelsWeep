using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Angel_Start_Place : NetworkBehaviour
{
    public Camera placementCamera;

    [SerializeField] private GameObject[] Angel_Teleport_pos;
    public List<Vector3> placedPositions = new List<Vector3>();
    [SerializeField] private float offset = 0.5f;
    private NetworkVariable<int> placedCount = new NetworkVariable<int>(0);
    [SerializeField] private int maxPlacements = 3;
    public LayerMask groundLayer;

    private PlayerInput pi;
    private InputAction placementAction;

    private Angel_Set_State controller;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        pi = GetComponent<PlayerInput>();
        placementAction = pi.actions["Placement"];
        placementAction.Enable();

        controller = GetComponent<Angel_Set_State>();
    }

    private void OnEnable()
    {
        if (placementCamera) placementCamera.enabled = true;
    }

    private void OnDisable()
    {
        if (placementCamera) placementCamera.enabled = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (placementAction.WasPressedThisFrame())
        {
            Ray ray = placementCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                PlaceObjectServerRpc(hit.point);
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            controller.StartGameplayServerRpc();
        }
        if (placedCount.Value >= maxPlacements)
            return;
    }

    [ServerRpc]
    private void PlaceObjectServerRpc(Vector3 position)
    {
        if (placedCount.Value >= maxPlacements)
            return;
        int index = placedCount.Value;
        if (index >= Angel_Teleport_pos.Length)
            return;

        Vector3 spawnPos = position + Vector3.up * offset;

        GameObject prefabToSpawn = Angel_Teleport_pos[index];

        GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
        placedPositions.Add(spawnPos);

        placedCount.Value++;
    }
}

