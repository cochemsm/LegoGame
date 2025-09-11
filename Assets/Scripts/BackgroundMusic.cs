using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour {
    [SerializeField] private List<AudioClip> music = new();
    
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
