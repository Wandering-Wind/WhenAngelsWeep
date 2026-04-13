using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class Seeker_Movement : NetworkBehaviour
{

    [Header("Player Components")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Torch")]
    public Light torchLight;
    public GameObject playerTorch;

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sneakAction;
    private InputAction torchAction;
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
        sneakAction = pi.actions["Sneak"];
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sneakAction.Enable();

        if (playerCamera) playerCamera.enabled = true;
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

        if (!IsOwner) return;
        if (torchAction.WasPressedThisFrame())
        {
            OnTorch();
        }
    }

    public void OnTorch()
    {
        if (!torchLight.enabled) return;
        RaycastHit hit;

        if (Physics.Raycast(playerTorch.transform.position, playerTorch.transform.forward, out hit, 20f))
        {
            if (hit.collider.CompareTag("Angel"))
            {
                print("Ahhh");
            }
        }

        Debug.DrawRay(playerTorch.transform.position, playerTorch.transform.forward * 20f, Color.red);
    }
    public void Torch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        torchLight.enabled = !torchLight.enabled;
    }
}

