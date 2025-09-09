using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerParenting : MonoBehaviour {
    private void Awake() {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("Player")) {
            other.transform.SetParent(null);
        }
    }
}
