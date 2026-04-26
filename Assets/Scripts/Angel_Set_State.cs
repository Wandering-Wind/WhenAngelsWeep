using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using static Angel_Set_State;

public class Angel_Set_State : NetworkBehaviour
{
    public Angel_Movment movement;
    public Angel_Start_Place placement;

    public enum GameState
    {
        Placement,
        Gameplay
    }

    public NetworkVariable<GameState> currentState = new NetworkVariable<GameState>(GameState.Placement);
    public override void OnNetworkSpawn()
    {
        currentState.OnValueChanged += OnStateChanged;
        ApplyState(currentState.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentState.OnValueChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        ApplyState(newState);
    }

    private void ApplyState(GameState state)
    {

        if (movement != null)
            movement.enabled = (state == GameState.Gameplay);

        if (placement != null)
            placement.enabled = (state == GameState.Placement);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameplayServerRpc()
    {
        currentState.Value = GameState.Gameplay;
    }
}


