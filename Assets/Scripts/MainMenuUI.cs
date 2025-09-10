using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour {
    private UIDocument _uiDocument;
    private VisualElement _root;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;
        
        _root.Q<Button>("PlayButton").clicked += Play;
        _root.Q<Button>("QuitButton").clicked += Quit;
    }

    private void Play() {
        SceneManager.LoadScene(1);
    }

    private void Quit() {
        Application.Quit();
    }
}
