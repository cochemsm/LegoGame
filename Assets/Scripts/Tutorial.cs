using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(BoxCollider))]
public class Tutorial : MonoBehaviour {
    private VisualElement _tutorialText;
    private bool _active;

    private void Awake() {
        _tutorialText = GetComponent<UIDocument>().rootVisualElement;
        _tutorialText.style.display = DisplayStyle.None;
    }

    private void OnTriggerEnter(Collider other) {
        if (_active) return;
        
        if (other.transform.CompareTag("Player")) {
            StartCoroutine(ShowTutorial());
            _active = true;
        }
    }

    private IEnumerator ShowTutorial() {
        _tutorialText.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(6);
        Destroy(gameObject);
    }
}
