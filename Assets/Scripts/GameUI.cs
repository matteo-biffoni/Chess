using System;
using System.Collections;
using ChessPieces;
using Cinemachine;
using Net;
using Net.NetMessages;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private ChessBoard ChessBoard;
    [SerializeField] private Transform Cameras;
    [SerializeField] private Animator MenuAnimator;
    [SerializeField] private TMP_InputField AddressInput;
    [SerializeField] private GameObject[] CameraAngles;
    public Action<bool> SetLocalGame;
    private static readonly int InGameMenu = Animator.StringToHash("InGameMenu");
    private static readonly int OnlineMenu = Animator.StringToHash("OnlineMenu");
    private static readonly int HostMenu = Animator.StringToHash("HostMenu");
    private static readonly int StartMenu = Animator.StringToHash("StartMenu");
    private bool _confrontationHandled = true;
    
    private CinemachineVirtualCamera _cameraBeforeConfrontation;

    private void Awake()
    {
        Instance = this;
        RegisterEvents();
    }

    private void Update()
    {
        if (!_confrontationHandled && _cameraBeforeConfrontation != null && Confrontation.GetCurrentOutcome() != Outcome.NotAvailable && !Confrontation.ConfrontationSceneLoaded)
        {
            _confrontationHandled = true;
            _cameraBeforeConfrontation.gameObject.SetActive(true);
            MainCamera.SetActive(true);
            EventSystem.SetActive(true);
            ZoomOut();
        }
    }

    private void ZoomOut()
    {
        ChessBoard.enabled = true;
        StartCoroutine(ZoomOutAndClean(4));
    }

    public void ZoomIn(ChessPiece defender, int turn)
    {
        ChessBoard.enabled = false;
        if (CameraAngles is not { Length: 3 })
        {
            Debug.LogError("Couldn't find a suitable camera to zoom in");
            return;
        }
        var newCamera = new GameObject("ZoomInCamera")
        {
            transform =
            {
                parent = Cameras
            }
        };
        newCamera.transform.position = turn switch
        {
            0 => CameraAngles[1].transform.position,
            1 => CameraAngles[2].transform.position,
            _ => newCamera.transform.position
        };
        newCamera.transform.LookAt(defender.transform);
        var virtualCamera = newCamera.AddComponent<CinemachineVirtualCamera>();
        foreach (var cameraAngle in CameraAngles)
        {
            cameraAngle.SetActive(false);
        }
        StartCoroutine(ZoomInAnimationAndChangeScene(virtualCamera, 10));
    }

    private IEnumerator ZoomOutAndClean(int speed)
    {
        while (Math.Abs(_cameraBeforeConfrontation.m_Lens.FieldOfView - 60f) > 1f)
        {
            _cameraBeforeConfrontation.m_Lens.FieldOfView = Mathf.Lerp(_cameraBeforeConfrontation.m_Lens.FieldOfView, 60f, speed * Time.deltaTime);
            yield return null;
        }
        _cameraBeforeConfrontation.gameObject.SetActive(false);
        _cameraBeforeConfrontation = null;
        switch (Confrontation.GetCurrentAttacking().Team)
        {
            case 0:
                CameraAngles[1].SetActive(true);
                break;
            case 1:
                CameraAngles[2].SetActive(true);
                break;
        }
        Confrontation.ResetConfrontation();
    }

    private IEnumerator ZoomInAnimationAndChangeScene(CinemachineVirtualCamera zoomInCamera, int speed)
    {
        _cameraBeforeConfrontation = zoomInCamera;
        var asyncScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        asyncScene.allowSceneActivation = false;
        while (Math.Abs(_cameraBeforeConfrontation.m_Lens.FieldOfView - .01f) > .001f)
        {
            _cameraBeforeConfrontation.m_Lens.FieldOfView = Mathf.Lerp(_cameraBeforeConfrontation.m_Lens.FieldOfView, .01f, speed * Time.deltaTime);
            yield return null;
        }
        asyncScene.allowSceneActivation = true;
        Confrontation.ConfrontationSceneLoaded = true;
        _cameraBeforeConfrontation.gameObject.SetActive(false);
        MainCamera.SetActive(false);
        EventSystem.SetActive(false);
        _confrontationHandled = false;
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
    // ReSharper disable once UnusedMember.Local
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
