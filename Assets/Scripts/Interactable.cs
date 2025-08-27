using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Animation))]
public class Interactable : MonoBehaviour {
    private SpriteRenderer _spriteRenderer;
    private Animation _animation;
    private Action _callback;

    [SerializeField] private Color color = Color.white;
    
    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        
        _spriteRenderer.color = color;
    }

    public void Interact(Action callback) {
        _callback = callback;
        StartCoroutine(InteractionProses());
    }

    private IEnumerator InteractionProses() {
        _animation.Play();
        yield return new WaitForSeconds(1);
        _callback.Invoke();
    }
}
