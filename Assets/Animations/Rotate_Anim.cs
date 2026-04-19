using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
public class Rotate_Anim : NetworkBehaviour
{
    private NetworkAnimator AnimR;

    public override void OnNetworkSpawn()
    {
        AnimR = GetComponent<NetworkAnimator>();

        if (IsServer)
        {
            AnimR.SetTrigger("Start_Rotate");
        }
    }

}
