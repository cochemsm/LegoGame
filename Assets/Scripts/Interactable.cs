using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(BoxCollider), typeof(Animation))]
public class Interactable : MonoBehaviour {
    private SpriteRenderer _spriteRenderer;
    private Animation _animation;
    private Action _callback;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    [SerializeField] private Minifigure character;
    [SerializeField] private AnimationClip SuccessClip;
    [SerializeField] private AnimationClip FailureClip;
    
    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        
        switch (character) {
            case Minifigure.All: _spriteRenderer.color = Color.white; break;
            case Minifigure.Guard: _spriteRenderer.color = new Color(1/255f, 53/255f, 100/255f, 1); break;
            case Minifigure.Janitor: _spriteRenderer.color = new Color(119/255f, 143/255f, 166/255f, 1); break;
            case Minifigure.Curator: _spriteRenderer.color = Color.white; break;
        }
        
        graph = PlayableGraph.Create();
    }

    private void Start() {
        AnimationPlayableOutput.Create(graph, "AnimOutput", GameManager.Player.Animator);
        graph.Stop();

        mixer = AnimationMixerPlayable.Create(graph, 2);
        graph.GetOutput(0).SetSourcePlayable(mixer);

        var clipSuccess = AnimationClipPlayable.Create(graph, SuccessClip);
        var clipFailure = AnimationClipPlayable.Create(graph, FailureClip);

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
        mixer.SetInputWeight(0, temp ? 1f : 0f);
        mixer.SetInputWeight(1, temp ? 0f : 1f);
        
        print("Success: " + temp);
        
        _callback = callback;
        StartCoroutine(InteractionProses());
    }

    private IEnumerator InteractionProses() {
        _animation.Play();
        graph.Play();
        yield return new WaitForSeconds(DetermineSuccess() ? SuccessClip.length : FailureClip.length);
        graph.Stop();
        _callback.Invoke();
    }

    private bool DetermineSuccess() {
        if (character == Minifigure.All) return true;
        return character == GameManager.Player.minifigure;
    }
}
