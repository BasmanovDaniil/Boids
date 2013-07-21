using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid
{
    public Vector3 velocity;
    public Vector3 position;

    
    private Boid[] boids;
    private Vector3 cohesion;
    private Vector3 separation;
    private int separationCount;
    private Vector3 alignment;

    public Boid(Vector3 pos)
    {
        position = pos;
    }
    public void CalculateVelocity()
    {
        Vector3 newVelocity = Vector3.zero;
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        separationCount = 0;
        alignment = Vector3.zero;
        boids = Boids.Sphere(position, Boids.cohesionRadius);
        foreach (Boid boid in boids)
        {
            alignment += boid.velocity;
            cohesion += boid.position;
            if (boid != this && (position - boid.position).magnitude < Boids.separationDistance)
            {
                separation += (position - boid.position) / (position - boid.position).magnitude;
                separationCount++;
            }
        }
        cohesion = cohesion / boids.Length;
        cohesion = cohesion - position;
        cohesion = Vector3.ClampMagnitude(cohesion, Boids.maxSpeed);

        if (separationCount > 0)
            separation = separation / separationCount;
        separation = Vector3.ClampMagnitude(separation, Boids.maxSpeed);

        alignment = alignment / boids.Length;
        alignment = Vector3.ClampMagnitude(alignment, Boids.maxSpeed);
        newVelocity += cohesion * 0.5f + separation * 15f + alignment * 1.5f;
        //newVelocity += 
        //    cohesion * Boids.cohesion + separation * Boids.separation + alignment * Boids.alignment;
        
        newVelocity = Vector3.ClampMagnitude(newVelocity, Boids.maxSpeed);
        velocity = (velocity * 2f + newVelocity) / 3f;
        if (position.magnitude > Boids.MaxDistance)
        {
            velocity += -position.normalized * 4f;
        }
    }
    
    public void Update()
    {
        position += velocity * Time.deltaTime;
        /**
        Debug.DrawRay(position, separation, Color.green);
        Debug.DrawRay(position, cohesion, Color.magenta);
        Debug.DrawRay(position, alignment, Color.blue);
        **/
    }
    
}
public class Boids : MonoBehaviour {

    GameObject[] boidsObjects;
    public int Count = 2500;
    public GameObject prefab;
    public static int MaxDistance = 50;
    public static float cohesionRadius = 10;
    public static float separationDistance = 5;
    public static float maxSpeed = 15;

    // Use this for initialization
	void Start () {
        boidsObjects = new GameObject[Count];
        boids = new Boid[boidsObjects.Length];
        for (int i = 0; i < boids.Length; i++)
        {
            GameObject boid = (GameObject)GameObject.Instantiate(prefab);
            boid.transform.position = Random.insideUnitSphere * (float)MaxDistance;
            boidsObjects[i] = boid;
            boids[i] = new Boid(boid.transform.position);
        }
        StartCoroutine(UpdatePhis());
	}
    float fps = 0f;
    void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));
        GUILayout.Box("FPS: " + fps.ToString("F2"));
        GUILayout.EndVertical();
    }
    IEnumerator UpdatePhis()
    {
        while (true)
        {
            int UpdateCount = 0;
            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].CalculateVelocity();
                UpdateCount++;
                if (UpdateCount > 100)
                //if (UpdateCount > 20)
                {
                    yield return null;
                    UpdateCount = 0;
                }
            }
        }
    }
	// Update is called once per frame
	void Update () {
        fps = (fps * 2f + 1f / Time.deltaTime) / 3f;
        Quaternion look;
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].Update();
            if (boids[i].velocity != Vector3.zero)
            {
                look = Quaternion.LookRotation(boids[i].velocity);
                boidsObjects[i].transform.rotation = Quaternion.Lerp(boidsObjects[i].transform.rotation,
                    look, Time.deltaTime * 2f);
            }
            boidsObjects[i].transform.position = boids[i].position;
        }
	}

    public static Boid[] boids;
    static public Boid[] Sphere(Vector3 position, float radius)
    {
        List<Boid> mids = new List<Boid>();
        for (int i = 0; i < boids.Length; i++)
        {
            if (Vector3.Distance(position, boids[i].position) < radius)
                mids.Add(boids[i]);
        }
        return mids.ToArray();
    }
}

