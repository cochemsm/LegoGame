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

    [SerializeField] private Color color = Color.white;
    [SerializeField] private AnimationClip playerAnimationClip;
    
    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        
        _spriteRenderer.color = color;
        
        graph = PlayableGraph.Create();
    }

    private void Start() {
        AnimationPlayableOutput.Create(graph, "AnimOutput", GameManager.Player.Animator);
        var playable = AnimationClipPlayable.Create(graph, playerAnimationClip);
        var output = graph.GetOutput(0);
        output.SetSourcePlayable(playable);
    }

    private void OnDestroy() {
        graph.Destroy();
    }

    public void Interact(Action callback) {
        graph.Play();
        _callback = callback;
        StartCoroutine(InteractionProses());
    }

    private IEnumerator InteractionProses() {
        _animation.Play();
        yield return new WaitForSeconds(1);
        graph.Stop();
        _callback.Invoke();
    }
}
