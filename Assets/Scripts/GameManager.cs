using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameData gameData;

    void Start() {
        SpawnPlayer(); // Spawn each player is joined

        // Spawn enemies if is the masterClient
        if (PhotonNetwork.IsMasterClient)
            SpawnEnemies();
    }

    void SpawnPlayer() {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector2.zero, Quaternion.identity);
    }

    void SpawnEnemies() {
        for (int i = 0; i < gameData.enemies.Count; i++) {
            GameObject enemyRef = PhotonNetwork.Instantiate(enemyPrefab.name, Vector2.one, Quaternion.identity);
            enemyRef.GetComponent<EnemyController>().SetUpEnemy(gameData.enemies[i]);
        }
    }
}
