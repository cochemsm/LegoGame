using System;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private Collider enterZone;
    [SerializeField] private Camera roomCamera;

    public Camera Camera => roomCamera;
    
    public event Action<Room> OnEnterRoom;

    public void Awake() {
        enterZone.isTrigger = true;
        Exit();
    }

    private void OnTriggerEnter(Collider other) {
        OnEnterRoom?.Invoke(this);
    }

    public void Exit() {
        roomCamera.gameObject.SetActive(false);
    }

    public void Enter() {
        roomCamera.gameObject.SetActive(true);
    }
}