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

    //[SerializeField] private Toggle FullBoardToggle;
    //[SerializeField] private GameObject PartialBoardPanel;
    //[SerializeField] private Toggle MiniGameToggle;
    //[SerializeField] private Toggle NumberOfTurnsToggle;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private ChessBoard ChessBoard;
    [SerializeField] private Transform Cameras;
    [SerializeField] private Animator MenuAnimator;
    [SerializeField] private TMP_InputField AddressInput;
    [SerializeField] private GameObject[] CameraAngles;
    [SerializeField] private GameObject PlaylistsMenu;
    [SerializeField] private GameObject CustomMatchMenu;
    [SerializeField] private Button ConnectButton;
    [SerializeField] private Image Image1, Image2;
    public Action<bool> SetLocalGame;
    private static readonly int InGameMenu = Animator.StringToHash("InGameMenu");
    private static readonly int HostMenu = Animator.StringToHash("HostMenu");
    private static readonly int StartMenu = Animator.StringToHash("StartMenu");
    private static readonly int WaitingMenu = Animator.StringToHash("WaitingMenu");
    private static readonly int FindAMatchMenu = Animator.StringToHash("FindMenu");
    private bool _confrontationHandled = true;
    private bool _playlistsMenu = true;
    private DispositionType _selectedAttackingDisposition = DispositionType.None;
    private DispositionType _selectedDefendingDisposition = DispositionType.None;
    [SerializeField] private Button[] DispositionButtonsAttacking;
    [SerializeField] private Button[] DispositionButtonsDefending;

    [SerializeField] private GameObject MovesPanel;

    [SerializeField] private GameObject MinigameStarting;
    [SerializeField] private GameObject MinigameOutcome;

    [SerializeField] private GameObject OptionsButton;
    [SerializeField] private GameObject OptionsMenu;

    [SerializeField] private GameObject SettingsMenu;
    
    private CinemachineVirtualCamera _cameraBeforeConfrontation;

    private bool _fullboard;
    private bool _2Turns;
    private bool _minigame = true;

    private bool _inGame;

    public void SetFullboard(bool value)
    {
        _fullboard = value;
    }

    public void Set2Turns(bool value)
    {
        _2Turns = value;
    }

    public void SetMinigame(bool value)
    {
        _minigame = value;
    }


    private void Awake()
    {
        Instance = this;
        RegisterEvents();
        for(var i = 0; i < DispositionButtonsAttacking.Length; i++)
        {
            var disposition = (DispositionType)(i+1);
            var buttonAttacking = DispositionButtonsAttacking[i];
            var buttonDefending = DispositionButtonsDefending[i];
            buttonAttacking.onClick.AddListener(delegate
            {
                AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
                SelectAttackingDisposition(disposition);
            });
            buttonDefending.onClick.AddListener(delegate
            {
                AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
                SelectDefendingDisposition(disposition);
            });
        }
    }

    private void ScaleButtons(bool attacking, int index)
    {
        if (attacking)
        {
            for (var i = 0; i < DispositionButtonsAttacking.Length; i++)
            {
                DispositionButtonsAttacking[i].transform.localScale =
                    i == index ? new Vector3(1.2f, 1.8f, 1.2f) : Vector3.one;
            }
        }
        else
        {
            for (var i = 0; i < DispositionButtonsDefending.Length; i++)
            {
                DispositionButtonsDefending[i].transform.localScale =
                    i == index ? new Vector3(1.2f, 1.8f, 1.2f) : Vector3.one;
            }
        }
    }

    public void SelectAttackingDisposition(DispositionType dispositionType)
    {
        _selectedAttackingDisposition = dispositionType;
        ScaleButtons(true, ((int) dispositionType - 1));
    }

    public DispositionType GetSelectedAttackingDisposition()
    {
        return _selectedAttackingDisposition;
    }

    private void SelectDefendingDisposition(DispositionType dispositionType)
    {
        _selectedDefendingDisposition = dispositionType;
        ScaleButtons(false, ((int) dispositionType - 1));
    }

    public void QuitEverything()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
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

        ConnectButton.interactable = AddressInput.text != "";
        if (_inGame && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!OptionsMenu.activeSelf)
                OpenOptionsMenu();
            else
                BackFromOptionsMenu();
        }
        
    }

    private void ZoomOut()
    {
        ChessBoard.enabled = true;
        var text = "";
        if (Confrontation.GetCurrentOutcome() == Outcome.Success)
        {
            text = Confrontation.GetCurrentAttacking().Team == 0 ? "Bug wins!" : "Vegetable wins!";
        }
        else if (Confrontation.GetCurrentOutcome() == Outcome.Failure)
        {
            text = Confrontation.GetCurrentAttacking().Team == 0 ? "Vegetable wins!" : "Bug wins!";
        }
        StartCoroutine(FadeInMinigameOutcome(text));
        StartCoroutine(ZoomOutAndClean(4));
    }

    private IEnumerator FadeMinigameStarting()
    {
        MinigameStarting.transform.localScale = Vector3.zero;
        MinigameStarting.SetActive(true);
        var timeElapsed = 0f;
        while (timeElapsed < 1.5f)
        {
            MinigameStarting.transform.localScale =
                Vector3.Lerp(MinigameStarting.transform.localScale, Vector3.one, timeElapsed / 1.5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        MinigameStarting.transform.localScale = Vector3.one;
        var color = Color.white;
        timeElapsed = 0f;
        while (timeElapsed < 0.3f)
        {
            color.a = Mathf.Lerp(color.a, 0f, timeElapsed / 0.3f);
            timeElapsed += Time.deltaTime;
            MinigameStarting.GetComponent<TMP_Text>().color = color;
            yield return null;
        }
        MinigameStarting.SetActive(false);
        color.a = 1f;
        MinigameStarting.GetComponent<TMP_Text>().color = color;
    }
    private IEnumerator FadeInMinigameOutcome(string text)
    {
        MinigameOutcome.GetComponent<TMP_Text>().text = text;
        MinigameOutcome.transform.localScale = Vector3.zero;
        MinigameOutcome.SetActive(true);
        var timeElapsed = 0f;
        while (timeElapsed < 1.5f)
        {
            MinigameOutcome.transform.localScale =
                Vector3.Lerp(MinigameOutcome.transform.localScale, Vector3.one, timeElapsed / 1.5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        MinigameOutcome.transform.localScale = Vector3.one;
        var color = Color.white;
        timeElapsed = 0f;
        while (timeElapsed < 0.3f)
        {
            color.a = Mathf.Lerp(color.a, 0f, timeElapsed / 0.3f);
            timeElapsed += Time.deltaTime;
            MinigameOutcome.GetComponent<TMP_Text>().color = color;
            yield return null;
        }
        MinigameOutcome.SetActive(false);
        color.a = 1f;
        MinigameOutcome.GetComponent<TMP_Text>().color = color;
    }

    public void ZoomIn(ChessPiece defender, bool isAttacking)
    {
        _inGame = false;
        OptionsButton.SetActive(false);
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
        StartCoroutine(FadeMinigameStarting());
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
        if (prevCamera != null) 
            prevCamera.SetActive(true);
        Confrontation.ResetConfrontation();
        MovesPanel.SetActive(true);
        OptionsButton.SetActive(true);
        _inGame = true;
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
    public void OnHostMenuButton()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        MenuAnimator.SetTrigger(HostMenu);
    }

    public void OnFindMenuButton()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        MenuAnimator.SetTrigger(FindAMatchMenu);
        SelectDefendingDisposition(DispositionType.Heavy);
    }
    public void OnHostHostButton()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        MatchConfiguration.SetGameUIConfigurationP1(_fullboard, _selectedAttackingDisposition, _2Turns, _minigame);
        Server.Init(8007);
        SetLocalGame?.Invoke(false);
        //_localGame = false;
        Client.Init("127.0.0.1", 8007);
        MenuAnimator.SetTrigger(WaitingMenu);
    }
    public void OnOnlineConnectButton()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        SetLocalGame?.Invoke(false);
        MatchConfiguration.SetGameUIConfigurationP2(_selectedDefendingDisposition);
        Client.Init(AddressInput.text, 8007);
    }
    public void BackOnMainMenu()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        MenuAnimator.SetTrigger(StartMenu);
    }
    public void OnWaitingBackButton()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        Server.Shutdown();
        Client.Shutdown();
        MenuAnimator.SetTrigger(HostMenu);
    }
    public void OnLeaveFromGameMenu()
    {
        _inGame = false;
        OptionsButton.SetActive(false);
        OptionsMenu.SetActive(false);
        ChangeCamera(CameraAngle.Menu);
        MenuAnimator.SetTrigger(StartMenu);
        StartCoroutine(FadeInPcs(2f));
    }

    public void OnPlaylistsButton(){
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        if(!_playlistsMenu){
            PlaylistsMenu.SetActive(true);
            PlaylistsMenu.transform.GetComponent<PlaylistsModeManager>().Visible = true;
            CustomMatchMenu.SetActive(false);
            CustomMatchMenu.transform.GetComponent<CustomModeManager>().Visible = false;
            _playlistsMenu = true;
        }
    }

    public void OnCustomMatchButton(){
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        if(_playlistsMenu){
            CustomMatchMenu.SetActive(true);
            CustomMatchMenu.transform.GetComponent<CustomModeManager>().Visible = true;
            PlaylistsMenu.SetActive(false);
            PlaylistsMenu.transform.GetComponent<PlaylistsModeManager>().Visible = false;
            _playlistsMenu = false;
        }
    }

    private IEnumerator FadeInPcs(float duration){
        var timeElapsed = 0f;
        var alphaStart = Image2.color.a;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            var color1 = Image1.color;
            var color2 = Image2.color;
            color1.a = Mathf.Lerp(0, 1, t);
            color2.a = Mathf.Lerp(alphaStart, 0, t);
            timeElapsed += Time.deltaTime;
            Image1.color = color1;
            Image2.color = color2;
            yield return null;
        }
    }

    public void OpenSettings()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        SettingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        SettingsMenu.SetActive(false);
    }

    public void SetGameBackgroundVolume(Single volume)
    {
        AudioManager.Instance.SetGameBackgroundVolume(volume);
    }

    public void SetMinigameBackgroundVolume(Single volume)
    {
        AudioManager.Instance.SetMinigameBackgroundVolume(volume);
    }

    public void SetGameEffectsVolume(Single volume)
    {
        AudioManager.Instance.SetGameEffectsVolume(volume);
    }

    public void SetMinigameEffectsVolume(Single volume)
    {
        AudioManager.Instance.SetMinigameEffectsVolume(volume);
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
        OptionsButton.SetActive(true);
        _inGame = true;
        StartCoroutine(FadeOutPcs(2f));
    }

    public void OpenOptionsMenu()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        OptionsMenu.SetActive(true);
        ChessBoard.SetPaused(true);
    }

    public void BackFromOptionsMenu()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        OptionsMenu.SetActive(false);
        ChessBoard.SetPaused(false);
    }


    private IEnumerator FadeOutPcs(float duration)
    {
        var timeElapsed = 0f;
        var alphaStart = Image2.color.a;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            var color1 = Image1.color;
            var color2 = Image2.color;
            color1.a = Mathf.Lerp(1, 0, t);
            color2.a = Mathf.Lerp(alphaStart, 0, t);
            timeElapsed += Time.deltaTime;
            Image1.color = color1;
            Image2.color = color2;
            yield return null;
        }
        var color1Fin = Image1.color;
        color1Fin.a = 0f;
        var color2Fin = Image2.color;
        color2Fin.a = 0f;
        Image1.color = color1Fin;
        Image2.color = color2Fin;
        StartCoroutine(ChessBoard.ShowTurnIndicator());
    }
    #endregion
}
