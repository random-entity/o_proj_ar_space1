using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    [SerializeField] private CloudPivot cloudPivotPrefab;
    [SerializeField] private int numberOfClouds;
    private CloudPivot[] cloudPivots;
    public static float minRotationSpeed = 0.25f, maxRotationSpeed = 4f;

    public static float getRandomSpeed()
    {
        return Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.Range(0f, 1f) < 0.5f ? 1f : -1f);
    }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        cloudPivots = new CloudPivot[numberOfClouds];

        for (int i = 0; i < numberOfClouds; i++)
        {
            CloudPivot ith = Instantiate(cloudPivotPrefab);
            cloudPivots[i] = ith;
            ith.transform.SetParent(transform);
        }
    }
}