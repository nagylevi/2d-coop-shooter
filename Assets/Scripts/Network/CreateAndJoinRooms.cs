using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks {

    public InputField joinInputField;
    public InputField createInputField;

    public void JoinRoom() {
        PhotonNetwork.JoinRoom(joinInputField.text);
    }

    public void CreateRoom() {
        PhotonNetwork.CreateRoom(createInputField.text);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel("Game");
    }

}
