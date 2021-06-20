using UnityEngine;

public class Gun : MonoBehaviour {

    public int damage;
    public float fireRate;
    public Transform firePoint;
    public Transform leftHandGrip;
    public Transform rightHandGrip;
    public GameObject impactParticle;
    public Animator animator;
    public LineRenderer lineRenderer;
}
