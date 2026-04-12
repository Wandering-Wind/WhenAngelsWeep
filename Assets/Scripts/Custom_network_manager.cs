using UnityEngine;
using Unity.Netcode;

public class Custom_network_manager : NetworkBehaviour
{

    public GameObject playerPrefabA;
    public GameObject playerPrefabB;

  /*  public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject prefabToUse;

        if (conn.connectionId % 2 == 0)
            prefabToUse = playerPrefabA;
        else
            prefabToUse = playerPrefabB;

        GameObject player = Instantiate(prefabToUse);

        NetworkServer.AddPlayerForConnection(conn, player);
    }*/
}
