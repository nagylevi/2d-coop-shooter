using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour {

    public GameObject playerPrefab;

    void Start() {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector2.zero, Quaternion.identity);
    }

}
