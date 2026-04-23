using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Angel_Set_State : NetworkBehaviour
{
    public Angel_Movment movement;
    public Angel_Start_Place placement;

    public enum GameState
    {
        Placement,
        Gameplay
    }

    private NetworkVariable<GameState> currentState = new NetworkVariable<GameState>(GameState.Placement);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        currentState.OnValueChanged += OnStateChanged;
        ApplyState(currentState.Value);
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        ApplyState(newState);
    }

    private void ApplyState(GameState state)
    {
        movement.enabled = (state == GameState.Gameplay);
        placement.enabled = (state == GameState.Placement);
    }

    [ServerRpc]
    public void StartGameplayServerRpc()
    {
        currentState.Value = GameState.Gameplay;
    }
}

