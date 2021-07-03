using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameData gameData;

    [SerializeField] private bool isTesting;

    void Start() {
        if (isTesting) {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to the server...");
        } else {
            // Spawn each player is joined
            SpawnPlayer();
            // Spawn enemies if is the masterClient
            if (PhotonNetwork.IsMasterClient)
                SpawnEnemies();
        }
    }

    void SpawnPlayer() {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector2.zero, Quaternion.identity);
    }

    void SpawnEnemies() {
        for (int i = 0; i < gameData.enemies.Count; i++) {
            GameObject enemyRef = PhotonNetwork.Instantiate(enemyPrefab.name, Vector2.one, Quaternion.identity);
            enemyRef.GetComponent<EnemyController>().SetUpEnemy(i);
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to the server!");
        PhotonNetwork.JoinLobby();
        Debug.Log("Joining to the lobby...");
    }

    public override void OnJoinedLobby() {
        Debug.Log("Joined to the lobby!");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", null, null);
        Debug.Log("Creating or Joining TestRoom...");
    }

    public override void OnJoinedRoom() {
        Debug.Log("Created or Joined TestRoom!");
        SpawnPlayer();
        Debug.Log("Spawned Player!");
        SpawnEnemies();
        Debug.Log("Spawned Enemies!");
    }
}
