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
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Teleport")]
    [SerializeField] public Transform Angel_pos_curr;
    [SerializeField] public GameObject Angel_pos_01;
    [SerializeField] public GameObject Angel_pos_02;
    [SerializeField] public GameObject Angel_pos_03;

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction killAction;
    private InputAction jumpAction;
    private InputAction teleportAction;
    private CharacterController cc;

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
        jumpAction = pi.actions["Jump"];
        killAction = pi.actions["Kill"];
        teleportAction = pi.actions["Teleport"];
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        killAction.Enable();
        teleportAction.Enable();

        if (playerCamera) playerCamera.enabled = true;
    }

    private void Start()
    {
        Angel_pos_curr = gameObject.GetComponent<Transform>();
    }
    private void Update()
    {
        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;
        cc.Move(move * moveSpeed * Time.deltaTime);

        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    public void OnKill(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Kill();
    }
    public void OnTeleport(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

    }
    public void Kill()
    {

    }
    public void Teleport()
    {

        transform.position = Angel_pos_01.transform.position;
        transform.position = Angel_pos_02.transform.position;
        transform.position = Angel_pos_03.transform.position;
    }
}


