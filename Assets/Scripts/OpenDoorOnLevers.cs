using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OpenDoorOnLevers : MonoBehaviour {
    [SerializeField] private List<Interactable> interactables = new();
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private float openSpeed;
    [SerializeField] private float openAngle;
    
    private bool _isOpen;

    private void Update() {
        if (_isOpen) return;
        
        if (interactables.TrueForAll(i => i.Finished)) {
            doorLeft.Rotate(Vector3.up, -openAngle);
            doorRight.Rotate(Vector3.up, openAngle);
            _isOpen = true;
        }
    }
}
