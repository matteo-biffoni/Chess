using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }
    private AudioSource _audioSource;

    [SerializeField] private AudioClip PickUpPiece;
    [SerializeField] private AudioClip PlaceDownPiece;



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
    PlaceDownPiece
}
