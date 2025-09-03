using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AnimationClip TransitionClip;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private bool InfiniteUse;
    [SerializeField] private List<Material> characterMaterials = new();

    private AnimationClipPlayable clipSuccess;
    private AnimationClipPlayable clipFailure;
    
    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        graph = PlayableGraph.Create();

        RoomManager.OnCameraChange += c => _camera = c.transform;
    }

    private void OnValidate() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _spriteRenderer.color = characterMaterials[(int)character].color;
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
        bool success = DetermineSuccess();
        clipSuccess.SetTime(0);
        clipFailure.SetTime(0);
        mixer.SetInputWeight(0, success ? 1f : 0f);
        mixer.SetInputWeight(1, success ? 0f : 1f);
        
        print("Success: " + success);
        
        _callback = callback;
        StartCoroutine(InteractionProses(success));
    }

    private IEnumerator InteractionProses(bool success) {
        Vector3 target = success ? _lookAtTarget.position : _camera.position;
        GameManager.Player.transform.LookAt(new Vector3(target.x, GameManager.Player.transform.position.y, target.z));
        graph.Play();
        if (success && transitionAnimator != null) transitionAnimator.Play("TransitionAnimation");
        yield return new WaitForSeconds(success ? SuccessClip.length : FailureClip.length);
        graph.Stop();
        _callback.Invoke();
        if (success && animator != null) animator.Play("InteractionAnimation");
        if (success && !InfiniteUse) Destroy(gameObject);
    }

    private bool DetermineSuccess() {
        if (character == Minifigure.All) return true;
        return character == GameManager.Player.minifigure;
    }
}
