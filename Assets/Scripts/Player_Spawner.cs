using Unity.Netcode;
using UnityEngine;

public class Player_Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject angelPrefab;
    [SerializeField] private GameObject seekerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void SpawnPlayer(ulong clientId)
    {
        // Decide role (you can change this logic later)
        bool isAngel = clientId % 2 == 0;

        GameObject prefab = isAngel ? angelPrefab : seekerPrefab;

        Vector3 spawnPos = GetSpawnPoint(isAngel);

        GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);

        player.GetComponent<NetworkObject>()
              .SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetSpawnPoint(bool isAngel)
    {
        return isAngel ? new Vector3(0, 1, 0) : new Vector3(5, 1, 5);
    }
}
