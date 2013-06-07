using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;

    private float cohesionRadius = 10;
    private float separationDistance = 5;
    private Collider[] boids;
    private Vector3 cohesion;
    private Vector3 separation;
    private int separationCount;
    private Vector3 alignment;
    private float maxSpeed = 15;

    private void Start()
    {
        InvokeRepeating("CalculateVelocity", 0, 0.1f);
    }

    void CalculateVelocity()
    {
        velocity = Vector3.zero;
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        separationCount = 0;
        alignment = Vector3.zero;

        boids = Physics.OverlapSphere(transform.position, cohesionRadius);
        foreach (var boid in boids)
        {
            cohesion += boid.transform.position;
            alignment += boid.GetComponent<Boid>().velocity;

            if (boid != collider && (transform.position - boid.transform.position).magnitude < separationDistance)
            {
                separation += (transform.position - boid.transform.position) / (transform.position - boid.transform.position).magnitude;
                separationCount++;
            }
        }

        cohesion = cohesion / boids.Length;
        cohesion = cohesion - transform.position;
        cohesion = Vector3.ClampMagnitude(cohesion, maxSpeed);
        if (separationCount > 0)
        {
            separation = separation / separationCount;
            separation = Vector3.ClampMagnitude(separation, maxSpeed);
        }
        alignment = alignment / boids.Length;
        alignment = Vector3.ClampMagnitude(alignment, maxSpeed);

        velocity += cohesion + separation * 10 + alignment * 1.5f;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }

    void Update()
    {
        if (transform.position.magnitude > 25)
        {
            velocity += -transform.position.normalized;
        }

        transform.position += velocity * Time.deltaTime;

        Debug.DrawRay(transform.position, separation, Color.green);
        Debug.DrawRay(transform.position, cohesion, Color.magenta);
        Debug.DrawRay(transform.position, alignment, Color.blue);
    }
}