using UnityEngine;
using Photon.Pun;

public class RigidbodySync : MonoBehaviourPun, IPunObservable {

    Rigidbody2D r;

    Vector3 latestPos;
    Quaternion latestRot;
    Vector2 velocity;
    float angularVelocity;

    bool valuesReceived = false;

    // Start is called before the first frame update
    void Start() {
        r = GetComponent<Rigidbody2D>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(r.velocity);
            stream.SendNext(r.angularVelocity);
        } else {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector2)stream.ReceiveNext();
            angularVelocity = (float)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!photonView.IsMine && valuesReceived) {
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 15);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 15);
            r.velocity = velocity;
            r.angularVelocity = angularVelocity;
        }
    }
}