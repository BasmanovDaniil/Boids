using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 anchor;
    public int turnSpeed = 10;
    public int maxSpeed = 15;
    public float cohesionRadius = 7;
    public int maxBoids = 10;
    public float separationDistance = 5;
    public float cohesionCoefficient = 1;
    public float alignmentCoefficient = 4;
    public float separationCoefficient = 10;
    public float tick = 2;
    public Transform model;
    public LayerMask boidsLayer;

    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Transform tr;

    private Collider[] boids;
    private Vector3 cohesion;
    private Vector3 separation;
    private int separationCount;
    private Vector3 alignment;

    private Boid b;
    private Vector3 vector;
    private int i;

    void Awake()
    {
        tr = transform;
        velocity = Random.onUnitSphere*maxSpeed;
    }

    private void Start()
    {
        InvokeRepeating("CalculateVelocity", Random.value * tick, tick);
        InvokeRepeating("UpdateRotation", Random.value, 0.1f);
    }

    void CalculateVelocity()
    {
        boids = Physics.OverlapSphere(tr.position, cohesionRadius, boidsLayer.value);
        if (boids.Length < 2) return;

        velocity = Vector3.zero;
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        separationCount = 0;
        alignment = Vector3.zero;
        
        for (i = 0; i < boids.Length && i < maxBoids; i++)
        {
            b = boids[i].GetComponent<Boid>();
            cohesion += b.tr.position;
            alignment += b.velocity;
            vector = tr.position - b.tr.position;
            if (vector.sqrMagnitude > 0 && vector.sqrMagnitude < separationDistance * separationDistance)
            {
                separation += vector / vector.sqrMagnitude;
                separationCount++;
            }
        }

        cohesion = cohesion / (boids.Length > maxBoids ? maxBoids : boids.Length);
        cohesion = Vector3.ClampMagnitude(cohesion - tr.position, maxSpeed);
        cohesion *= cohesionCoefficient;
        if (separationCount > 0)
        {
            separation = separation / separationCount;
            separation = Vector3.ClampMagnitude(separation, maxSpeed);
            separation *= separationCoefficient;
        }
        alignment = alignment / (boids.Length > maxBoids ? maxBoids : boids.Length);
        alignment = Vector3.ClampMagnitude(alignment, maxSpeed);
        alignment *= alignmentCoefficient;

        velocity = Vector3.ClampMagnitude(cohesion + separation + alignment, maxSpeed);
    }

    void UpdateRotation()
    {
        if (velocity != Vector3.zero && model.forward != velocity.normalized)
        {
            model.forward = Vector3.RotateTowards(model.forward, velocity, turnSpeed, 1);
        }
    }

    void Update()
    {
        if (Vector3.Distance(tr.position, anchor) > 25)
        {
           velocity += (anchor - tr.position) / 25;
        }
        tr.position += velocity * Time.deltaTime;
    }
}
