using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    [SerializeField] private List<Room> rooms;

    private Room currentRoom;

    public static event Action<CinemachineCamera> OnCameraChange;
    
    [Obsolete("Better use the OnCameraChange Event, so camera is not fetched every frame")]
    public static CinemachineCamera currentCamera => instance.currentRoom.Camera;

    private static RoomManager instance;
    
    private void Awake() {
        instance = this;
        foreach (var room in rooms) {
            room.OnEnterRoom += OnRoomEnter;
        }
    }

    private void Start() {
        SwitchToRoom(rooms[0]);
    }

    private void OnRoomEnter(Room room) {
        if (currentRoom == room) return;

        SwitchToRoom(room);
    }

    private void SwitchToRoom(Room room) {
        if(currentRoom != null) currentRoom.Exit();
        room.Enter();
        currentRoom = room;
        OnCameraChange?.Invoke(currentRoom.Camera);
    }
}