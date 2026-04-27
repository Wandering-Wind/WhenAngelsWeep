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
    [SerializeField] private float currMoveSpeed = 1f;
    public float changeSpeed;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Speed Change")]
    public float FakeSpeedIncreaseVar = 1;
    public float TimeSpeedIncreaseVar = 1;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource creakSource;
    [SerializeField] private AudioClip creakSound; // Your WeepingAngelMoving.mp3

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private CharacterController cc;

    [SerializeField] private float speedTimeInc = 180f;
    [SerializeField] private float freezeGraceTime = 0.2f;
    private NetworkVariable<bool> isFrozen = new NetworkVariable<bool>(false);

    private float pitch;
    private bool wasMovingLastFrame = false;

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

        // Setup audio source if not assigned
        if (creakSource == null)
        {
            creakSource = gameObject.AddComponent<AudioSource>();
            creakSource.playOnAwake = false;
            creakSource.spatialBlend = 1f; // 3D sound so others can hear it
            creakSource.volume = 1f;
            creakSource.loop = false;
        }
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
        {
            cc.Move(Vector3.zero);
            wasMovingLastFrame = false;
            return;
        }

        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;

        cc.Move(move * currMoveSpeed * Time.deltaTime);

        // Check if currently moving
        bool isMovingNow = m.magnitude > 0.1f;

        // Play sound when movement starts (transition from stopped to moving)
        if (isMovingNow && !wasMovingLastFrame)
        {
            PlayCreakSound();
        }

        wasMovingLastFrame = isMovingNow;

        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    private void PlayCreakSound()
    {
        if (creakSource != null && creakSound != null && !creakSource.isPlaying)
        {
            creakSource.PlayOneShot(creakSound);

            // Sync to other players (so they can hear the killer moving)
            if (IsServer)
            {
                PlayCreakClientRpc();
            }
        }
    }

    [ClientRpc]
    private void PlayCreakClientRpc()
    {
        if (!IsOwner && creakSource != null && creakSound != null)
        {
            creakSource.PlayOneShot(creakSound);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TimeChangeSpeedServerRpc()
    {
        StartCoroutine(SpeedIncreaseRoutine());
    }

    public void FakeSpeedInc()
    {
        print("GVHJO{KJIHVJJO{OIYFGUOHIPJ{}HOGUIOHP");
        currMoveSpeed += FakeSpeedIncreaseVar;
    }

    public void Freeze()
    {
        SetFreezeServerRpc(true);
    }

    public void Unfreeze()
    {
        SetFreezeServerRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetFreezeServerRpc(bool state)
    {
        isFrozen.Value = state;
    }

    private IEnumerator SpeedIncreaseRoutine()
    {
        yield return new WaitForSeconds(speedTimeInc);
        currMoveSpeed += TimeSpeedIncreaseVar;
    }

}


