using UnityEngine;

public class BuildingSystem : MonoSingleton<BuildingSystem>
{
    #region Fields for Buildings config
    public Building[] buildings;
    public Material[] materials;
    private Building sky, buildingToSky, doorToBuilding, zeroToDoor, unseen1, unseen2;
    [SerializeField] private Windows[] windowsSets;
    #endregion

    #region Fields for Buildings drawing
    public static float centerDir = Mathf.PI * 0.5f; // = 0 degrees // 빌딩의 중심축이 xz 평면에서 어떤 argument(각) 갖는지. 
    public static float[] presetAngWids;
    public static float angWidStepRatio = 16f;
    public static float raduisRatio = 1.1f;
    public static float angWidToHeight = 1.5f;
    #endregion

    #region Fields for processing player control input
    private float playerSpeed = 0.3f;
    private float playerWalk = 0f; // 1f가 되면 한 페이즈 종료.
    private int iteration = 0;
    #endregion

    private void Start()
    {
        buildings = GetComponentsInChildren<Building>();
        if (buildings.Length != 6) Debug.LogWarning("Number of buildings != 6. Check Hierarchy.");
        if (materials.Length != 3) Debug.LogWarning("Number of materials != 3. Check Inspector.");
        if (windowsSets.Length != 2) Debug.LogWarning("Number of Windows Sets != 2. Check Inspector.");

        presetAngWids = new float[4];
        presetAngWids[0] = 2f * Mathf.PI;
        for (int i = 1; i < presetAngWids.Length; i++)
        {
            presetAngWids[i] = presetAngWids[i - 1] / angWidStepRatio;
        }

        for (int i = 0; i < buildings.Length; i++)
        {
            buildings[i].GetComponent<MeshRenderer>().material = materials[i % 3];
        }

        SetBuildingsRoleIndices();
        WalkAndUpdate(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            WalkAndUpdate(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            WalkAndUpdate(false);
        }
    }

    private void SetBuildingsRoleIndices()
    {
        sky = buildings[iteration];
        buildingToSky = buildings[(iteration + 1) % 6];
        doorToBuilding = buildings[(iteration + 2) % 6];
        zeroToDoor = buildings[(iteration + 3) % 6];
        unseen1 = buildings[(iteration + 4) % 6];
        unseen2 = buildings[(iteration + 5) % 6];

        sky.gameObject.SetActive(true);
        buildingToSky.gameObject.SetActive(true);
        doorToBuilding.gameObject.SetActive(true);
        zeroToDoor.gameObject.SetActive(true);
        unseen1.gameObject.SetActive(false);
        unseen2.gameObject.SetActive(false);
    }
    private void UpdateBuildingsMesh()
    {
        sky.UpdateMesh(playerWalk, 0);
        buildingToSky.UpdateMesh(playerWalk, 1);
        doorToBuilding.UpdateMesh(playerWalk, 2);
        zeroToDoor.UpdateMesh(playerWalk, 3);

        windowsSets[0].SetMeshes(buildingToSky, doorToBuilding);
        windowsSets[0].StartFades(true);
    }
    private void WalkAndUpdate(bool forward)
    {
        playerWalk += (forward ? 1f : -1f) * playerSpeed * Time.deltaTime;
        if (playerWalk >= 1f)
        {
            playerWalk %= 1f;
            iteration++;
        }
        else if (playerWalk < 0f)
        {
            playerWalk += 1f;
            iteration += 5;
        }

        if (iteration >= 6)
        {
            iteration %= 6;
        }
        SetBuildingsRoleIndices();

        UpdateBuildingsMesh();
    }
}