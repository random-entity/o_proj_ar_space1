using UnityEngine;

public class EnvSpecs : MonoSingleton<EnvSpecs>
{
    public static float landRadius = 50f;
    [SerializeField] private Transform groundCylinder;
    [SerializeField] private Transform roadCube;

    private void Start()
    {
        groundCylinder.localScale = new Vector3(2 * landRadius, 0.1f, 2 * landRadius);
        roadCube.localScale = new Vector3(1f, 0.1f, 2 * landRadius);
    }
}