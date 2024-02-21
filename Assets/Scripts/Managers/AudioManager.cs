using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip[] sfxClips;

    private void Awake()
    {
        bgmPlayer.clip = bgmClip;
    }

    public void PlayBGM()
    {
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySfx(int index)
    {
        sfxPlayer.PlayOneShot(sfxClips[index]);
    }

    void Start()
    {
        PlayBGM();
    }
}
