using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class Interactable : MonoBehaviour {
    private SpriteRenderer _spriteRenderer;
    private Action _callback;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;
    private Transform _camera;
    [SerializeField] private Transform _lookAtTarget;

    [SerializeField] private Minifigure character;
    [SerializeField] private AnimationClip SuccessClip;
    [SerializeField] private AnimationClip FailureClip;
    [SerializeField] private Animator animator;

    private AnimationClipPlayable clipSuccess;
    private AnimationClipPlayable clipFailure;
    
    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        graph = PlayableGraph.Create();

        RoomManager.OnCameraChange += c => _camera = c.transform;
    }

    private void OnValidate() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        switch (character) {
            case Minifigure.All: _spriteRenderer.color = Color.white; break;
            case Minifigure.Guard: _spriteRenderer.color = new Color(1/255f, 53/255f, 100/255f, 1); break;
            case Minifigure.Janitor: _spriteRenderer.color = new Color(58/255f, 84/255f, 108/255f, 1); break;
            case Minifigure.Curator: _spriteRenderer.color = new Color(137/255f, 76/255f, 36/255f, 1); break;
        }
    }

    private void Start() {
        AnimationPlayableOutput.Create(graph, "AnimOutput", GameManager.Player.Animator);
        graph.Stop();

        mixer = AnimationMixerPlayable.Create(graph, 2);
        graph.GetOutput(0).SetSourcePlayable(mixer);
        
        mixer.SetTraversalMode(PlayableTraversalMode.Mix);
        mixer.SetPropagateSetTime(true);

        clipSuccess = AnimationClipPlayable.Create(graph, SuccessClip);
        clipFailure = AnimationClipPlayable.Create(graph, FailureClip);

        graph.Connect(clipSuccess, 0, mixer, 0);
        graph.Connect(clipFailure, 0, mixer, 1);
        
        mixer.SetInputWeight(0, 0f);
        mixer.SetInputWeight(1, 1f);
    }

    private void OnDestroy() {
        graph.Destroy();
    }

    public void Interact(Action callback) {
        bool temp = DetermineSuccess();
        clipSuccess.SetTime(0);
        clipFailure.SetTime(0);
        mixer.SetInputWeight(0, temp ? 1f : 0f);
        mixer.SetInputWeight(1, temp ? 0f : 1f);
        
        print("Success: " + temp);
        
        _callback = callback;
        StartCoroutine(InteractionProses());
    }

    private IEnumerator InteractionProses() {
        Vector3 target = DetermineSuccess() ? _lookAtTarget.position : _camera.position;
        GameManager.Player.transform.LookAt(new Vector3(target.x, GameManager.Player.transform.position.y, target.z));
        graph.Play();
        yield return new WaitForSeconds(DetermineSuccess() ? SuccessClip.length : FailureClip.length);
        graph.Stop();
        _callback.Invoke();
        if (DetermineSuccess()) animator.Play("InteractionAnimation");
    }

    private bool DetermineSuccess() {
        if (character == Minifigure.All) return true;
        return character == GameManager.Player.minifigure;
    }
}
