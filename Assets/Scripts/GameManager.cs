using UnityEngine;
using ThirdPersonPlayerController;

public class GameManager : MonoBehaviour {
    public static Player Player { get; private set; }

    private void Awake() {
        Player = FindAnyObjectByType<Player>();
    }
}
