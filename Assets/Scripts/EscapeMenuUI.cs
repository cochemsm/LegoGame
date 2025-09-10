using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EscapeMenuUI : MonoBehaviour {
    private bool _isPaused;
    private VisualElement _pauseMenu;
    
    private void Start() {
        InputSystem.actions.FindAction("Escape").Enable();
        _pauseMenu = GetComponent<UIDocument>().rootVisualElement;
        _pauseMenu.style.display = DisplayStyle.None;

        _pauseMenu.Q<Button>("ResumeButton").clicked += Unpause;
        _pauseMenu.Q<Button>("ExitButton").clicked += () => {
            Unpause();
            SceneManager.LoadScene(0);
        };
    }

    private void OnEnable() {
        InputSystem.actions.FindAction("Escape").performed += TogglePauseMenu;
    }

    private void OnDisable() {
        InputSystem.actions.FindAction("Escape").performed -= TogglePauseMenu;
    }

    private void TogglePauseMenu(InputAction.CallbackContext ctx) {
        if (_isPaused) Unpause();
        else Pause();
    }

    private void Pause() {
        _isPaused = true;
        Time.timeScale = 0f;
        _pauseMenu.style.display = DisplayStyle.Flex;
    }

    private void Unpause() {
        _isPaused = false;
        Time.timeScale = 1f;
        _pauseMenu.style.display = DisplayStyle.None;
    }
}
