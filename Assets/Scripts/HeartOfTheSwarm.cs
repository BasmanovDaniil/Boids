using UnityEngine;

public class HeartOfTheSwarm : MonoBehaviour
{
    public Transform boidPrefab;
    public int swarmCount = 100;


    void Start()
	{
        for (var i = 0; i < swarmCount; i++)
        {
            Instantiate(boidPrefab, Random.insideUnitSphere * 25, Quaternion.identity);
        }
    }
}
