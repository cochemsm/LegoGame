using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour {
    private static BackgroundMusic Instance;
    [SerializeField] private List<AudioClip> music = new();
    private int _currentClip;
    private AudioSource _source;
    private Action _clipFinished;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _source = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        _clipFinished = () => StartCoroutine(PlayMusic());
        _clipFinished?.Invoke();
    }

    private IEnumerator PlayMusic() {
        _source.PlayOneShot(music[_currentClip]);
        yield return new WaitForSeconds(music[_currentClip].length);
        _currentClip++;
        if (_currentClip == music.Count) _currentClip = 0;
        _clipFinished?.Invoke();
    }
}
