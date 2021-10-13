using UnityEngine;

public class BuildingSystemGPU : MonoBehaviour
{
    #region fields for Buildings config
    private BuildingGPU[] buildings;
    [SerializeField] private BuildingGPU buildingPrefab;
    private BuildingGPU sky, buildingToSky, doorToBuilding, zeroToDoor, unseen1, unseen2;
    private WindowGroup[] windowGroups;
    [SerializeField] private WindowGroup windowGroupPrefab;
    #endregion

    #region fields for Buildings geometry & color
    public static float centerDir = Mathf.PI * 0.5f; // 빌딩의 중심축이 xz 평면에서 어떤 argument(각) 갖는지.
    public static float[] presetAngWids;
    public static float angWidStepRatio = 16f;
    public static float raduisStepRatio = 1.1f;
    public static float angWidToHeight = 1.5f;
    private static Color[] colors;
    private Color getColorFromIndex(int colorIndex)
    {
        if (colorIndex < 0)
        {
            Debug.Log("getColorFromIndex() : argument < 0");
            return Color.magenta;
        }
        if (colorIndex >= totalColor)
        {
            colorIndex %= totalColor;
        }
        return colors[colorIndex];
    }
    #endregion

    #region fields for processing player control input
    private int currSkyBldgIndex = 0;
    private static int totalBldg = 3 * 2;
    private int currSkyColorIndex = 0;
    private static int totalColor = 4;
    private int currSkyWinGrpIndex = 0;
    private int totalWinGrp = 3;
    #endregion

    private void Awake()
    {
        Extensions.InitializeArray<BuildingGPU>(out buildings, buildingPrefab, 6, transform);
        Extensions.InitializeArray<WindowGroup>(out windowGroups, windowGroupPrefab, 3, transform, (wg) => { wg.InitializeAllAlpha(0f); });
        windowGroups[1].InitializeAllAlpha(1f); // buildingToSky (match to buildingRoleIndex)

        presetAngWids = new float[4];
        presetAngWids[0] = 2f * Mathf.PI;
        for (int i = 1; i < presetAngWids.Length; i++)
        {
            presetAngWids[i] = presetAngWids[i - 1] / angWidStepRatio;
        }

        colors = PaletteContainer.instance.palette.colors;
        if (colors.Length != BuildingSystemGPU.totalColor)
        {
            Debug.LogWarning("palette.Length != BuildingSystem.totalColor");
        }

        SetBuildingsRoleIndicesAndColorAndRenderQueueAndActive();
        UpdateBuildingsAndWindowGroupsMesh();
    }

    #region event listening methods and event subscriptions
    void OnWalkerStageSwitch(bool forward)
    {
        int add = forward ? 1 : -1;

        Extensions.ModuloAdd(ref currSkyBldgIndex, add, totalBldg);
        Extensions.ModuloAdd(ref currSkyColorIndex, add, totalColor);
        Extensions.ModuloAdd(ref currSkyWinGrpIndex, add, totalWinGrp);

        if (forward)
        {
            windowGroups[currSkyWinGrpIndex].StartFades(false);
            windowGroups[(currSkyWinGrpIndex + 1) % 3].StartFades(true);
        }
        else
        {
            windowGroups[(currSkyWinGrpIndex + 1) % 3].StartFades(true);
            windowGroups[(currSkyWinGrpIndex + 2) % 3].StartFades(false);
        }
        SetBuildingsRoleIndicesAndColorAndRenderQueueAndActive();
    }
    private void OnWalkerProgress()
    {
        UpdateBuildingsAndWindowGroupsMesh();
    }
    private void OnEnable()
    {
        Walker.ProgressAlert += OnWalkerProgress;
        Walker.StageSwitchAlert += OnWalkerStageSwitch;
    }
    private void OnDisable()
    {
        Walker.ProgressAlert -= OnWalkerProgress;
        Walker.StageSwitchAlert -= OnWalkerStageSwitch;
    }
    #endregion

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
        sky = buildings[currSkyBldgIndex];
        buildingToSky = buildings[(currSkyBldgIndex + 1) % 6];
        doorToBuilding = buildings[(currSkyBldgIndex + 2) % 6];
        zeroToDoor = buildings[(currSkyBldgIndex + 3) % 6];
        unseen1 = buildings[(currSkyBldgIndex + 4) % 6];
        unseen2 = buildings[(currSkyBldgIndex + 5) % 6];
    }
    private void SetBuildingsColor()
    {
        sky.SetMaterialColor(getColorFromIndex(currSkyColorIndex));
        buildingToSky.SetMaterialColor(getColorFromIndex(currSkyColorIndex + 1));
        doorToBuilding.SetMaterialColor(getColorFromIndex(currSkyColorIndex + 2));
        zeroToDoor.SetMaterialColor(getColorFromIndex(currSkyColorIndex + 3));
    }
    private void SetBuildingsRenderQueue()
    {
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
        sky.UpdateMeshByWalkProgress(Walker.progress, 0);
        buildingToSky.UpdateMeshByWalkProgress(Walker.progress, 1);
        doorToBuilding.UpdateMeshByWalkProgress(Walker.progress, 2);
        zeroToDoor.UpdateMeshByWalkProgress(Walker.progress, 3);
    }
    private void UpdateWindowGroupsMesh()
    {
        // windowGroups[currSkyWinGrpIndex].SetMeshes(sky, buildingToSky);
        // windowGroups[(currSkyWinGrpIndex + 1) % 3].SetMeshes(buildingToSky, doorToBuilding);
        // windowGroups[(currSkyWinGrpIndex + 2) % 3].SetMeshes(doorToBuilding, zeroToDoor);
    }
    #endregion
}