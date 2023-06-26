using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource GameAudioSource;
    [SerializeField] private AudioSource MinigameAudioSource;

    [SerializeField] private AudioClip PickUpPiece;
    [SerializeField] private AudioClip PlaceDownPiece;
    [SerializeField] private AudioClip NormalAttack;
    [SerializeField] private AudioClip SpecialAttack;
    [SerializeField] private AudioClip ButtonPressed;

    [SerializeField] private AudioSource GameBackgroundAudioSource;
    private AudioSource _minigameBackgroundAudioSource;
    private float _minigameBackgroundAudioVolume = 1f;



    public void PlayClip(SoundClip which)
    {
        var minigame = false;
        switch (which)
        {
            case SoundClip.PickUpPiece:
                GameAudioSource.clip = PickUpPiece;
                break;
            case SoundClip.PlaceDownPiece:
                GameAudioSource.clip = PlaceDownPiece;
                break;
            case SoundClip.NormalAttack:
                MinigameAudioSource.clip = NormalAttack;
                minigame = true;
                break;
            case SoundClip.SpecialAttack:
                MinigameAudioSource.clip = SpecialAttack;
                minigame = true;
                break;
            case SoundClip.ButtonPressed:
                GameAudioSource.clip = ButtonPressed;
                break;
        }

        if (minigame)
            MinigameAudioSource.Play();
        else
            GameAudioSource.Play();
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SetGameBackgroundVolume(float volume)
    {
        GameBackgroundAudioSource.volume = volume;
    }

    public void SetMinigameBackgroundVolume(float volume)
    {
        _minigameBackgroundAudioVolume = volume;
        if (_minigameBackgroundAudioSource != null)
        {
            _minigameBackgroundAudioSource.volume = _minigameBackgroundAudioVolume;
        }
    }

    public void SetMinigameBackgroundSource(AudioSource source)
    {
        _minigameBackgroundAudioSource = source;
        _minigameBackgroundAudioSource.volume = _minigameBackgroundAudioVolume;
    }

    public void SetGameEffectsVolume(float volume)
    {
        GameAudioSource.volume = volume;
    }

    public void SetMinigameEffectsVolume(float volume)
    {
        MinigameAudioSource.volume = volume;
    }
}

public enum SoundClip
{
    PickUpPiece,
    PlaceDownPiece,
    NormalAttack,
    SpecialAttack,
    ButtonPressed
}
