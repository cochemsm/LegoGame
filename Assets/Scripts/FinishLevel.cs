using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            SceneManager.LoadScene(0);
        }
    }
}
