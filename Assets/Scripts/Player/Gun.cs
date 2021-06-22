using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("Gun Stats")]
    public int damage;
    public float fireRate;
    public GunFireMode fireMode;

    [Header("IK Targets")]
    public Transform leftHandGrip;
    public Transform rightHandGrip;

    [Header("References")]
    public Transform firePoint;
    public GameObject impactParticle;
    public Animator animator;
    public LineRenderer lineRenderer;

    private bool readyToShoot = true;

    public void Shoot() {
        readyToShoot = false;
        Invoke(nameof(ResetShoot), fireRate);
    }

    void ResetShoot() {
        readyToShoot = true;
    }

    public bool IsReadyToShoot() {
        return readyToShoot;
    }
}

public enum GunFireMode {
    Automatic,
    Single,
}
