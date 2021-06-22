using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroWithTime : MonoBehaviour {

    public float timeToDestroy;
    private float timer;
    private PhotonView view;
    private bool isNetworkedObj;

    void Start()
    {
        timer = timeToDestroy;
        view = GetComponent<PhotonView>();
        if (view != null) {
            isNetworkedObj = true;
        } else {
            isNetworkedObj = false;
        }
    }

    void Update() {
        if (timer < 0) {
            if (isNetworkedObj) {
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.Destroy(view);
                }
            } else {
                Destroy(gameObject);
            }
        } else {
            timer -= Time.deltaTime;
        }
    }
}
