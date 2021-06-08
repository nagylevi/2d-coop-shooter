using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour {

    private Text loadingText;
    private float timer;
    private readonly float timeToStep = 0.5f;
    private int startingLength;
    private string startingText;

    private void Start() {
        loadingText = GetComponent<Text>();
        timer = timeToStep;
        startingLength = loadingText.text.Length;
        startingText = loadingText.text;
    }

    private void Update() {
        if (timer < 0) {
            if (loadingText.text.Length != startingLength) {
                loadingText.text = startingText;
            } else {
                loadingText.text = loadingText.text.Remove(loadingText.text.Length - 1);
            }
            timer = timeToStep;
        } else {
            timer -= Time.deltaTime;
        }
    }

}
