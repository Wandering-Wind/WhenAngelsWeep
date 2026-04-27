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

    [SerializeField] private Mesh[] artifactMeshes;
    [SerializeField] private GameObject realArtifactPrefab;
    [SerializeField] private GameObject fakeArtifactPrefab;

    private NetworkVariable<int> artifactPlacedCount = new NetworkVariable<int>(0);
    private int maxArtifacts = 3;

    private PlayerInput pi;
    private InputAction placementAction;

    [SerializeField] private Angel_Set_State controller;

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
                if(placedCount.Value < maxPlacements)
                {
                    PlaceAngelServerRpc(hit.point);
                }
                else if (artifactPlacedCount.Value < maxArtifacts)
                {
                    PlaceObjectServerRpc(hit.point);
                }
                
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            controller.StartGameplayServerRpc();
        }

    }

    [ServerRpc]
    private void PlaceAngelServerRpc(Vector3 position)
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

    [ServerRpc]
    private void PlaceObjectServerRpc(Vector3 position)
    {
        if (artifactPlacedCount.Value >= maxArtifacts)
            return;

        Vector3 spawnPos = position + Vector3.up * offset;

        GameObject prefabToSpawn;

        if (artifactPlacedCount.Value < 2)
        {
            prefabToSpawn = fakeArtifactPrefab;
        }
        else
        {
            prefabToSpawn = realArtifactPrefab;
        }

        GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        var netObj = obj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("No_Artfiact");
            return;
        }
        if (artifactMeshes.Length > 0)
        {
            int randomIndex = Random.Range(0, artifactMeshes.Length);

            MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
            if (mf != null)
            {
                mf.mesh = artifactMeshes[randomIndex];
            }
        }
        netObj.Spawn();

        artifactPlacedCount.Value++;
    }
}

