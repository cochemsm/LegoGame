using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorOnLevers : MonoBehaviour {
    [SerializeField] private List<Interactable> interactables = new();
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private float openSpeed = 1f;
    [SerializeField] private float openAngle;
    
    private bool _isOpen;

    private void Update() {
        if (_isOpen) return;
        
        if (interactables.TrueForAll(i => i.Finished)) {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor() {
        yield return new WaitForSeconds(1f);
        _isOpen = true;
        float currentAngle = 0;
        while (currentAngle < openAngle) {
            currentAngle += Time.deltaTime * openSpeed;
            doorLeft.localRotation = Quaternion.Euler(0, -currentAngle, 0);
            doorRight.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }
        
}
