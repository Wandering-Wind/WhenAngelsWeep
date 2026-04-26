using System.Collections;
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

    [Header("Player Settings")]
    [SerializeField] private float currMoveSpeed = 5f;
    public float changeSpeed;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Speed Change")]
    public float FakeSpeedIncreaseVar = 1;
    public float TimeSpeedIncreaseVar = 1;


    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private CharacterController cc;

    [SerializeField] private float speedTimeInc = 180f;
    [SerializeField] private float freezeGraceTime = 0.2f;
    private NetworkVariable<bool> isFrozen = new NetworkVariable<bool>(false);

    private float pitch;

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
        moveAction.Enable();
        lookAction.Enable();
        TimeChangeSpeedServerRpc();

    }
    private void OnEnable()
    {
        if (playerCamera) playerCamera.enabled = true;
    }

    private void OnDisable()
    {
        if (playerCamera) playerCamera.enabled = false;
    }


    private void Update()
    {

        if (!IsOwner) return;
        if (isFrozen.Value)
            return;
        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;

        cc.Move(move * currMoveSpeed * Time.deltaTime);

        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

    }

    [ServerRpc(RequireOwnership = false)]
    private void TimeChangeSpeedServerRpc()
    {
        StartCoroutine(SpeedIncreaseRoutine()); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void FakeSpeedIncServerRpc()
    {
        currMoveSpeed += FakeSpeedIncreaseVar;
    }

    [ServerRpc(RequireOwnership = false)]
    public void FreezeServerRpc()
    {
        StartCoroutine(FreezeRoutine());
        Debug.Log("FROZEN on server");
    }

    private IEnumerator SpeedIncreaseRoutine()
    {

        yield return new WaitForSeconds(speedTimeInc);
        currMoveSpeed += TimeSpeedIncreaseVar;
    }
    private IEnumerator FreezeRoutine()
    {
        isFrozen.Value = true;
        yield return new WaitForSeconds(freezeGraceTime);
        isFrozen.Value = false;
    }

}


