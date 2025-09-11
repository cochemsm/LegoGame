using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class FinishLevel : MonoBehaviour {
    private VisualElement _endScreen;

    private void Awake() {
        _endScreen = GetComponent<UIDocument>().rootVisualElement;
        _endScreen.style.display = DisplayStyle.None;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            StartCoroutine(ShowEndScreen());
        }
    }

    private IEnumerator ShowEndScreen() {
        Cursor.visible = true;
        InputSystem.actions.FindAction("Move").Disable();
        InputSystem.actions.FindAction("Escape").Disable();
        InputSystem.actions.FindAction("Change").Disable();
        InputSystem.actions.FindAction("Jump").Disable();
        _endScreen.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(0);
    }
}
