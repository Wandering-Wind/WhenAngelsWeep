using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport;

public class Custom_network_manager : MonoBehaviour
{
    [SerializeField] private GameObject angelPrefab;
    [SerializeField] private GameObject seekerPrefab;

    public void SpawnPlayer(ulong clientId, bool isAngel)
    {
        GameObject prefab = isAngel ? angelPrefab : seekerPrefab;

        GameObject player = Instantiate(prefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
