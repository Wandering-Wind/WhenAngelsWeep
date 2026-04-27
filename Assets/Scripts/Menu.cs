using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private string Scene;

    [Header("Defaults")]
    [SerializeField] private string defaultIP = "127.0.0.1";
    [SerializeField] private ushort defaultPort = 7777;

    [SerializeField] private UnityTransport transport;
    [SerializeField] private NetworkManager networkManager;

    private void Awake()
    {
        if (ipInput) ipInput.text = defaultIP;
        if (portInput) portInput.text = defaultPort.ToString();

    }
    public void StartHost()
    {
        ushort port = GetPort();
        transport.SetConnectionData("0.0.0.0", port); // listen on all interfaces

        // set connection data first ...
        networkManager.StartHost();
        // Host loads game scene for everyone
        networkManager.SceneManager.LoadScene(Scene, LoadSceneMode.Single);
    }
    public void JoinGame()
    {
        string ip = GetIPC();
        ushort port = GetPort();

        transport.SetConnectionData(ip, port);
        networkManager.StartClient();
    }
    public void StartServerOnly()
    {
        ushort port = GetPort();
        transport.SetConnectionData("0.0.0.0", port);
        networkManager.StartServer();
    }

    private string GetIPC()
    {

        if (!ipInput || string.IsNullOrWhiteSpace(ipInput.text))
            return defaultIP;

        return ipInput.text.Trim();

    }

    private ushort GetPort()
    {

        if (!portInput || !ushort.TryParse(portInput.text, out ushort port))
            return defaultPort;

        return port;
    }
}
