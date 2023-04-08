using System;
using Net;
using Net.NetMessages;
using TMPro;
using UnityEngine;

public enum CameraAngle
{
    Menu = 0,
    WhiteTeam = 1,
    BlackTeam = 2
}

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    public Server Server;
    public Client Client;

    [SerializeField] private Animator MenuAnimator;
    [SerializeField] private TMP_InputField AddressInput;
    [SerializeField] private GameObject[] CameraAngles;
    public Action<bool> SetLocalGame;
    private static readonly int InGameMenu = Animator.StringToHash("InGameMenu");
    private static readonly int OnlineMenu = Animator.StringToHash("OnlineMenu");
    private static readonly int HostMenu = Animator.StringToHash("HostMenu");
    private static readonly int StartMenu = Animator.StringToHash("StartMenu");

    private void Awake()
    {
        Instance = this;
        RegisterEvents();
    }
    public void ChangeCamera(CameraAngle index)
    {
        foreach (var cameraAngle in CameraAngles)
            cameraAngle.SetActive(false);
        CameraAngles[(int) index].SetActive(true);
    }
    public void OnLocalGameButton()
    {
        MenuAnimator.SetTrigger(InGameMenu);
        SetLocalGame?.Invoke(true);
        Server.Init(8007);
        Client.Init("127.0.0.1", 8007);
    }
    public void OnOnlineGameButton()
    {
        MenuAnimator.SetTrigger(OnlineMenu);
    }
    public void OnOnlineHostButton()
    {
        Server.Init(8007);
        SetLocalGame?.Invoke(false);
        Client.Init("127.0.0.1", 8007);
        MenuAnimator.SetTrigger(HostMenu);
    }
    public void OnOnlineConnectButton()
    {
        SetLocalGame?.Invoke(false);
        Client.Init(AddressInput.text, 8007);
    }
    public void OnOnlineBackButton()
    {
        MenuAnimator.SetTrigger(StartMenu);
    }
    public void OnHostBackButton()
    {
        Server.Shutdown();
        Client.Shutdown();
        MenuAnimator.SetTrigger(OnlineMenu);
    }
    public void OnLeaveFromGameMenu()
    {
        ChangeCamera(CameraAngle.Menu);
        MenuAnimator.SetTrigger(StartMenu);
    }
    #region MULTIPLAYER
    private void RegisterEvents()
    {
        NetUtility.CStartGame += OnStartGameClient;
    }
    private void UnregisterEvents()
    {
        NetUtility.CStartGame -= OnStartGameClient;
    }
    private void OnStartGameClient(NetMessage msg)
    {
        MenuAnimator.SetTrigger(InGameMenu);
    }
    #endregion
}
