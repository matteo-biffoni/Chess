using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _audioSource;

    [SerializeField] private AudioClip PickUpPiece;
    [SerializeField] private AudioClip PlaceDownPiece;
    [SerializeField] private AudioClip NormalAttack;
    [SerializeField] private AudioClip SpecialAttack;



    public void PlayClip(SoundClip which)
    {
        switch (which)
        {
            case SoundClip.PickUpPiece:
                _audioSource.clip = PickUpPiece;
                break;
            case SoundClip.PlaceDownPiece:
                _audioSource.clip = PlaceDownPiece;
                break;
            case SoundClip.NormalAttack:
                _audioSource.clip = NormalAttack;
                break;
            case SoundClip.SpecialAttack:
                _audioSource.clip = SpecialAttack;
                break;
        }
        _audioSource.Play();
    }

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }
}

public enum SoundClip
{
    PickUpPiece,
    PlaceDownPiece,
    NormalAttack,
    SpecialAttack
}
