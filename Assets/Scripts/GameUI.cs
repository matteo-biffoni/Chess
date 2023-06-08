using System;
using System.Collections;
using System.Linq;
using ChessPieces;
using Cinemachine;
using Net;
using Net.NetMessages;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] private Toggle FullBoardToggle;
    [SerializeField] private GameObject PartialBoardPanel;
    [SerializeField] private Toggle MiniGameToggle;
    [SerializeField] private Toggle NumberOfTurnsToggle;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private ChessBoard ChessBoard;
    [SerializeField] private Transform Cameras;
    [SerializeField] private Animator MenuAnimator;
    [SerializeField] private TMP_InputField AddressInput;
    [SerializeField] private GameObject[] CameraAngles;
    public Action<bool> SetLocalGame;
    private static readonly int InGameMenu = Animator.StringToHash("InGameMenu");
    private static readonly int HostMenu = Animator.StringToHash("HostMenu");
    private static readonly int StartMenu = Animator.StringToHash("StartMenu");
    private static readonly int WaitingMenu = Animator.StringToHash("WaitingMenu");
    private static readonly int FindAMatchMenu = Animator.StringToHash("FindMenu");
    private bool _confrontationHandled = true;
    private DispositionType _selectedAttackingDisposition = DispositionType.None;
    private DispositionType _selectedDefendingDisposition = DispositionType.None;
    [SerializeField] private Button[] DispositionButtonsAttacking;
    [SerializeField] private Button[] DispositionButtonsDefending;

    [SerializeField] private GameObject MovesPanel;
    //private bool _localGame;
    
    private CinemachineVirtualCamera _cameraBeforeConfrontation;

    private void Awake()
    {
        Instance = this;
        RegisterEvents();
        FullBoardToggle.onValueChanged.AddListener(delegate
        {
            PartialBoardPanel.SetActive(!FullBoardToggle.isOn);
            if (!FullBoardToggle.isOn) {
                SelectAttackingDisposition(DispositionType.Heavy);
                ScaleButtons(DispositionButtonsAttacking[0]);
            }
            else
            {
                SelectAttackingDisposition(DispositionType.None);
                ScaleButtons(null);
            }
        });
        for(var i = 0; i < DispositionButtonsAttacking.Length; i++)
        {
            var disposition = (DispositionType)(i+1);
            var buttonAttacking = DispositionButtonsAttacking[i];
            var buttonDefending = DispositionButtonsDefending[i];
            buttonAttacking.onClick.AddListener(delegate
            {
                SelectAttackingDisposition(disposition);
                ScaleButtons(buttonAttacking);
            });
            buttonDefending.onClick.AddListener(delegate
            {
                SelectDefendingDisposition(disposition);
                ScaleButtons(buttonDefending);
            });
        }
    }

    private void ScaleButtons(Button highlighted)
    {
        foreach (var button in DispositionButtonsAttacking)
            button.transform.localScale = button == highlighted ? new Vector3(1.4f, 1.4f, 1f) : new Vector3(1f, 1f, 1f);
        foreach (var button in DispositionButtonsDefending)
            button.transform.localScale = button == highlighted ? new Vector3(1.4f, 1.4f, 1f) : new Vector3(1f, 1f, 1f);
    }

    private void SelectAttackingDisposition(DispositionType dispositionType)
    {
        _selectedAttackingDisposition = dispositionType;
    }

    private void SelectDefendingDisposition(DispositionType dispositionType)
    {
        _selectedDefendingDisposition = dispositionType;
    }

    public void QuitEverything()
    {
        Application.Quit();
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

    public void ZoomIn(ChessPiece defender, bool isAttacking)
    {
        MovesPanel.SetActive(false);
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
        /*if (_localGame)
        {
            newCamera.transform.position = defender.Team switch
            {
                0 => CameraAngles[2].transform.position,
                1 => CameraAngles[1].transform.position,
                _ => newCamera.transform.position
            };
        }
        else*/
        {
            foreach (var cameraAngle in CameraAngles)
            {
                if (cameraAngle.activeSelf)
                {
                    newCamera.transform.position = cameraAngle.transform.position;
                    break;
                }
            }
        }
        newCamera.transform.LookAt(defender.transform);
        var virtualCamera = newCamera.AddComponent<CinemachineVirtualCamera>();
        foreach (var cameraAngle in CameraAngles)
        {
            cameraAngle.SetActive(false);
        }
        StartCoroutine(ZoomInAnimationAndChangeScene(virtualCamera, 10, isAttacking));
    }

    private IEnumerator ZoomOutAndClean(int speed)
    {
        while (Math.Abs(_cameraBeforeConfrontation.m_Lens.FieldOfView - 60f) > 1f)
        {
            _cameraBeforeConfrontation.m_Lens.FieldOfView = Mathf.Lerp(_cameraBeforeConfrontation.m_Lens.FieldOfView, 60f, speed * Time.deltaTime);
            yield return null;
        }
        _cameraBeforeConfrontation.gameObject.SetActive(false);
        var prevCamera = CameraAngles.FirstOrDefault(cameraAngle => cameraAngle.transform.position == _cameraBeforeConfrontation.transform.position);
        _cameraBeforeConfrontation = null;
        /*if (_localGame)
            switch (Confrontation.GetCurrentAttacking().Team)
            {
                case 0:
                    CameraAngles[1].SetActive(true);
                    break;
                case 1:
                    CameraAngles[2].SetActive(true);
                    break;
            }
        else */if (prevCamera != null) 
            prevCamera.SetActive(true);
        Confrontation.ResetConfrontation();
        MovesPanel.SetActive(true);
    }

    private IEnumerator ZoomInAnimationAndChangeScene(CinemachineVirtualCamera zoomInCamera, int speed, bool isAttacking)
    {
        _cameraBeforeConfrontation = zoomInCamera;
        ConfrontationListener.IsAttacking = isAttacking;
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
    /*public void OnLocalGameButton()
    {
        MenuAnimator.SetTrigger(InGameMenu);
        //SetLocalGame?.Invoke(true);
        //_localGame = true;
        Server.Init(8007);
        Client.Init("127.0.0.1", 8007);
    }*/
    public void OnHostMenuButton()
    {
        MenuAnimator.SetTrigger(HostMenu);
        SelectAttackingDisposition(DispositionType.Heavy);
        ScaleButtons(DispositionButtonsAttacking[0]);
    }

    public void OnFindMenuButton()
    {
        MenuAnimator.SetTrigger(FindAMatchMenu);
        SelectDefendingDisposition(DispositionType.Heavy);
        ScaleButtons(DispositionButtonsDefending[0]);
    }
    public void OnHostHostButton()
    {
        MatchConfiguration.SetGameUIConfigurationP1(FullBoardToggle.isOn, _selectedAttackingDisposition, NumberOfTurnsToggle.isOn, MiniGameToggle.isOn);
        Server.Init(8007);
        SetLocalGame?.Invoke(false);
        //_localGame = false;
        Client.Init("127.0.0.1", 8007);
        MenuAnimator.SetTrigger(WaitingMenu);
    }
    public void OnOnlineConnectButton()
    {
        SetLocalGame?.Invoke(false);
        //_localGame = false;
        MatchConfiguration.SetGameUIConfigurationP2(_selectedDefendingDisposition);
        Client.Init(AddressInput.text, 8007);
    }
    public void BackOnMainMenu()
    {
        MenuAnimator.SetTrigger(StartMenu);
    }
    public void OnWaitingBackButton()
    {
        Server.Shutdown();
        Client.Shutdown();
        MenuAnimator.SetTrigger(HostMenu);
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
