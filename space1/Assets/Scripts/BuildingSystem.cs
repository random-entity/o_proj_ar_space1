using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    #region fields for Buildings config
    private Building[] buildings;
    [SerializeField] private Building buildingPrefab;
    private Building sky, buildingToSky, doorToBuilding, zeroToDoor, unseen1, unseen2;
    private WindowGroup[] windowGroups;
    [SerializeField] private WindowGroup windowGroupPrefab;
    private int currSkyWinGrpIndex = 0;
    #endregion

    #region fields for Buildings geometry
    public static float centerDir = Mathf.PI * 0.5f; // 빌딩의 중심축이 xz 평면에서 어떤 argument(각) 갖는지.
    public static float[] presetAngWids;
    public static float angWidStepRatio = 16f;
    public static float raduisStepRatio = 1.1f;
    public static float angWidToHeight = 1.5f;
    #endregion

    #region fields for processing player control input
    private float playerSpeed = 0.3f;
    private float playerWalk = 0f; // 1f가 되면 한 페이즈 종료.
    private int currStep = 0; // private static int totalStep = 3 * 2;
    private int currDayPhase = 0;
    public static int totalDayPhase = 4;
    private static Color[] palette;
    private Color getColorFromDayPhase(int dayPhase)
    {
        if (dayPhase < 0)
        {
            Debug.Log("getColorFromDayPhase() : argument < 0");
            return Color.magenta;
        }

        else return palette[dayPhase % palette.Length];
    }
    #endregion

    #region helper functions
    private void initiateArray<T>(out T[] array, int size, T prefab) where T : MonoBehaviour
    {
        array = new T[size];
        for (int i = 0; i < size; i++)
        {
            T ith = Instantiate(prefab);
            array[i] = ith;
            ith.transform.SetParent(transform);
        }
    }
    #endregion

    private void Awake()
    {
        initiateArray<Building>(out buildings, 6, buildingPrefab);
        initiateArray<WindowGroup>(out windowGroups, 3, windowGroupPrefab);
        windowGroups[0].InitiateAllAlpha(0f); // sky (match buildingRoleIndex)
        windowGroups[1].InitiateAllAlpha(1f); // buildingToSky (match buildingRoleIndex)
        windowGroups[2].InitiateAllAlpha(0f); // doorToBuilding (match buildingRoleIndex)

        presetAngWids = new float[4];
        presetAngWids[0] = 2f * Mathf.PI;
        for (int i = 1; i < presetAngWids.Length; i++)
        {
            presetAngWids[i] = presetAngWids[i - 1] / angWidStepRatio;
        }

        palette = Palette.instance.palette;

        SetBuildingsRoleIndicesAndColorAndRenderQueueAndActive();
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

    private void WalkAndUpdate(bool forward)
    {
        Walk(forward);

        UpdateBuildingsAndWindowGroupsMesh();
    }
    private void Walk(bool forward)
    {
        playerWalk += (forward ? 1f : -1f) * playerSpeed * Time.deltaTime;

        if (playerWalk >= 1f || playerWalk < 0f)
        {
            if (playerWalk >= 1f)
            {
                playerWalk %= 1f;

                currStep++;
                currDayPhase++;
                currSkyWinGrpIndex = (currSkyWinGrpIndex + 1) % 3;

                windowGroups[currSkyWinGrpIndex].StartFades(false);
                windowGroups[(currSkyWinGrpIndex + 1) % 3].StartFades(true);
            }
            else // if (playerWalk < 0f)
            {
                playerWalk += 1f;

                currStep += 5;
                currDayPhase += totalDayPhase - 1;
                currSkyWinGrpIndex = (currSkyWinGrpIndex + 2) % 3;

                windowGroups[(currSkyWinGrpIndex + 1) % 3].StartFades(true);
                windowGroups[(currSkyWinGrpIndex + 2) % 3].StartFades(false);
            }

            if (currStep >= 6)
            {
                currStep %= 6;
            }
            if (currDayPhase >= totalDayPhase)
            {
                currDayPhase %= totalDayPhase;
            }

            SetBuildingsRoleIndicesAndColorAndRenderQueueAndActive();
        }
    }

    #region set buildings RoleIndices / Color / RenderQueue / Active (call when step change)
    private void SetBuildingsRoleIndicesAndColorAndRenderQueueAndActive()
    {
        SetBuildingsRoleIndices();
        SetBuildingsColor();
        SetBuildingsRenderQueue();
        SetBuildingsActive();
    }
    private void SetBuildingsRoleIndices()
    {
        sky = buildings[currStep];
        buildingToSky = buildings[(currStep + 1) % 6];
        doorToBuilding = buildings[(currStep + 2) % 6];
        zeroToDoor = buildings[(currStep + 3) % 6];
        unseen1 = buildings[(currStep + 4) % 6];
        unseen2 = buildings[(currStep + 5) % 6];
    }
    private void SetBuildingsColor()
    {
        sky.SetMaterialColor(getColorFromDayPhase(currDayPhase));
        buildingToSky.SetMaterialColor(getColorFromDayPhase(currDayPhase + 1));
        doorToBuilding.SetMaterialColor(getColorFromDayPhase(currDayPhase + 2));
        zeroToDoor.SetMaterialColor(getColorFromDayPhase(currDayPhase + 3));
    }
    private void SetBuildingsRenderQueue()
    {
        sky.SetRenderQueue(0);
        buildingToSky.SetRenderQueue(1);
        doorToBuilding.SetRenderQueue(2);
        zeroToDoor.SetRenderQueue(3);
    }
    private void SetBuildingsActive()
    {
        sky.gameObject.SetActive(true);
        buildingToSky.gameObject.SetActive(true);
        doorToBuilding.gameObject.SetActive(true);
        zeroToDoor.gameObject.SetActive(true);
        unseen1.gameObject.SetActive(false);
        unseen2.gameObject.SetActive(false);
    }
    #endregion
    
    #region set buildings & windowGroups Mesh (call every frame where user moves)
    private void UpdateBuildingsAndWindowGroupsMesh()
    {
        UpdateBuildingsMesh();
        UpdateWindowGroupsMesh();
    }
    private void UpdateBuildingsMesh()
    {
        sky.UpdateMesh(playerWalk, 0);
        buildingToSky.UpdateMesh(playerWalk, 1);
        doorToBuilding.UpdateMesh(playerWalk, 2);
        zeroToDoor.UpdateMesh(playerWalk, 3);
    }
    private void UpdateWindowGroupsMesh()
    {
        windowGroups[currSkyWinGrpIndex].SetMeshes(sky, buildingToSky);
        windowGroups[(currSkyWinGrpIndex + 1) % 3].SetMeshes(buildingToSky, doorToBuilding);
        windowGroups[(currSkyWinGrpIndex + 2) % 3].SetMeshes(doorToBuilding, zeroToDoor);
    }
    #endregion
}