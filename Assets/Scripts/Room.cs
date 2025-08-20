using System;
using Unity.Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private Collider enterZone;
    [SerializeField] private CinemachineCamera roomCamera;

    public CinemachineCamera Camera => roomCamera;
    public event Action<Room> OnEnterRoom;

    public void Awake() {
        enterZone.isTrigger = true;
        Exit();
    }

    private void OnTriggerEnter(Collider other) {
        OnEnterRoom?.Invoke(this);
    }

    public void Exit() {
        roomCamera.Follow = transform;
        roomCamera.gameObject.SetActive(false);
    }

    public void Enter() {
        roomCamera.Follow = GameManager.Player.transform;
        roomCamera.gameObject.SetActive(true);
    }
}