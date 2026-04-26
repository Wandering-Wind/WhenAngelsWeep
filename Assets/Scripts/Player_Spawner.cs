using Unity.Netcode;
using UnityEngine;

public class Player_Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject angelPrefab;
    [SerializeField] private GameObject seekerPrefab;
    [SerializeField] private Transform SeekerSpawn;
    [SerializeField] private Transform AngelSpawn;
    private Vector3 seekerpos;
    private Vector3 Angelpos;

    private int playerCount = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        seekerpos = SeekerSpawn.transform.position;
        Angelpos = AngelSpawn.transform.position;
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (angelPrefab == null || seekerPrefab == null)
        {
            return;
        }
        bool isAngel = playerCount == 0;
        playerCount++;

        GameObject prefab = isAngel ? angelPrefab : seekerPrefab;

        Vector3 spawnPos = GetSpawnPoint(isAngel);

        GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetSpawnPoint(bool isAngel)
    {
        return isAngel ? Angelpos : seekerpos;
    }
}
