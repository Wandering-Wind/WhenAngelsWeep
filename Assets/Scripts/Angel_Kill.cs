using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Angel_Kill : NetworkBehaviour
{
    [Header("Kill Settings")]
    public Transform AngelKillpos;
    private PlayerInput pi;
    private InputAction killAction;
    [SerializeField] private float killRange = 3f;
    [SerializeField] private GameObject winScreenAngel;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        pi = GetComponent<PlayerInput>();
        killAction = pi.actions["Interact"];
        killAction.Enable();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (killAction.WasPressedThisFrame())
        {
            TryKill();
        }
    }
    private void TryKill()
    {
        Ray ray = new Ray(AngelKillpos.position, AngelKillpos.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, killRange))
        {
            NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();

            if (netObj != null)
            {
                KillServerRpc(netObj.NetworkObjectId);
            }
        }
    }
    [ServerRpc]
    public void KillServerRpc(ulong objectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            if (netObj.CompareTag("Seeker"))
            {
                Instantiate(winScreenAngel);
                Time.timeScale = 0f;
                Debug.Log("GameOver");
              

            }
        }
    }
}
