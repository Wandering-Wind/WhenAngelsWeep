using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class Angel_Movment : NetworkBehaviour
{

    [Header("Player Components")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [Header("Placement Camera")]
    [SerializeField] private Camera placementCamera;
    [SerializeField] private GameObject placeablePrefab;
    [SerializeField] private LayerMask groundLayer;

    [Header("Player Settings")]
    [SerializeField] private float currMoveSpeed = 5f;
    public float changeSpeed;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Speed Change")]
    public Seeker_Interact interactChange;

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction placementAction;
    private CharacterController cc;
    private MeshRenderer meshAngel;

    private float lastHitTime;
    [SerializeField] private float freezeGraceTime = 0.2f;
    private NetworkVariable<bool> isFrozen = new NetworkVariable<bool>(false);

    private float pitch;

    public enum GameState
    {
        Placement,
        Gameplay
    }
    private NetworkVariable<GameState> currentState = new NetworkVariable<GameState>(GameState.Placement);
    public override void OnNetworkSpawn()
    {
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();

        if (!IsOwner)
        {
            if (playerCamera) playerCamera.enabled = false;
            if (pi) pi.enabled = false;
            enabled = false;
            return;
        }

        moveAction = pi.actions["Move"];
        lookAction = pi.actions["Look"];
        placementAction = pi.actions["Placement"];
        moveAction.Enable();
        lookAction.Enable();
        placementAction.Enable();

        meshAngel = gameObject.GetComponent<MeshRenderer>();

        currentState.OnValueChanged += OnStateChanged;
        UpdateCameraState(currentState.Value);
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        UpdateCameraState(newState);
    }

    private void UpdateCameraState(GameState state)
    {
        if (state == GameState.Placement)
        {
            meshAngel.enabled = false;
            placementCamera.enabled = true;
            playerCamera.enabled = false;
        }
        else
        {
            meshAngel.enabled = true;
            placementCamera.enabled = false;
            playerCamera.enabled = true;
        }
    }


    private void Update()
    {

        if (!IsOwner) return;
        if (currentState.Value == GameState.Placement)
        {
            HandlePlacement();
            return;
        }
        if (isFrozen.Value)
            return;
        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;

        //TimeChangeSpeedServerRpc();
        interactChange.ChangeAngelSpeed(changeSpeed);

        cc.Move(move * currMoveSpeed * Time.deltaTime);

        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

        if (Time.time - lastHitTime > freezeGraceTime)
        {
            isFrozen.Value = false;
        }
    }
    private void HandlePlacement()
    {
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
            StartGameplayServerRpc();
        }
    }
    [ServerRpc]
    private void PlaceObjectServerRpc(Vector3 position)
    {
        GameObject obj = Instantiate(placeablePrefab, position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    private void StartGameplayServerRpc()
    {
        currentState.Value = GameState.Gameplay;
    }

    /*[ServerRpc]
    private void TimeChangeSpeedServerRpc()
    {
        currMoveSpeed += changeSpeed; 
    }*/

    [ServerRpc]
    public void FreezeServerRpc()
    {
        lastHitTime = Time.time;
        isFrozen.Value = true;
    }

}


