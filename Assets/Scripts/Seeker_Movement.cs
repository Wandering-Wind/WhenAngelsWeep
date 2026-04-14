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
    [SerializeField] private float currmoveSpeed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sneakSpeed = 3f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Vector3 velocity;

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sneakAction;
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
        currmoveSpeed = moveSpeed;
    }

    private void Update()
    {
        ApplyGravity();
        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;
        cc.Move(move * currmoveSpeed * Time.deltaTime);

        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

        if (jumpAction.WasPressedThisFrame()) 
        {
            OnJumpServerRpc();
        }        
        if(sneakAction.WasPressedThisFrame())
        {
            OnSneakServerRpc();
        }
        if(sneakAction.WasReleasedThisFrame())
        {
            currmoveSpeed = moveSpeed; 
        }
    }
   
     [ServerRpc]
     public void OnJumpServerRpc()
     {
         if(cc.isGrounded)
         {
             velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
         }
     }

    [ServerRpc]
    public void OnSneakServerRpc()
    {
        if(cc.isGrounded)
        {
            currmoveSpeed = sneakSpeed;
        }
    }
     private void ApplyGravity()
     {
         if (cc.isGrounded && velocity.y < 0)
             velocity.y = -2f;

         velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
     }
}


