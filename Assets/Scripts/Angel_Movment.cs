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

    }


    private void Update()
    {

        if (!IsOwner) return;
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

    }

    /*[ServerRpc]
    private void TimeChangeSpeedServerRpc()
    {
        currMoveSpeed += changeSpeed; 
    }*/

    [ServerRpc]
    public void FreezeServerRpc()
    {
        StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        isFrozen.Value = true;
        yield return new WaitForSeconds(freezeGraceTime);
        isFrozen.Value = false;
    }

}


