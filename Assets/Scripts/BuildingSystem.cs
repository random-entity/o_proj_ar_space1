using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    #region fields for Buildings config
    private Building[] buildings;
    [SerializeField] private Building buildingPrefab;
    private Building sky, buildingToSky, doorToBuilding, zeroToDoor, unseen1, unseen2;
    public static readonly int renderQueueSky = 3000, renderQueuebuildingToSky = 3002, renderQueuedoorToBuilding = 3004, renderQueuezeroToDoor = 3005;
    #endregion

    #region fields for Buildings geometry & color
    public static float angWidStepRatio = 16f;
    private static Color[] colors;
    #endregion

    #region index fields for processing player control input
    private int currSkyBldgIndex = 0;
    private static int totalBldg = 3 * 2;
    private int currSkyColorIndex = 0;
    private static int totalColor = 4;
    #endregion

    private void Awake()
    {
        Extensions.InitializeArray<Building>(out buildings, buildingPrefab, 6, transform);
        buildings[1].InitializeWindowAlphas(1f);

        colors = PaletteContainer.instance.palette.colors;
        if (colors.Length != BuildingSystem.totalColor)
        {
            Debug.LogWarning("palette.Length != BuildingSystem.totalColor");
        }

        SetBuildingsRoleIndexAndColorAndRenderQueueAndActive();
        UpdateBuildingsMesh();
    }

    #region event listening methods and event subscriptions
    void OnWalkerStageSwitch(bool forward)
    {
        int add = forward ? 1 : -1;

        Extensions.SafeModuloAdd(ref currSkyBldgIndex, add, totalBldg);
        Extensions.SafeModuloAdd(ref currSkyColorIndex, add, totalColor);

        SetBuildingsRoleIndexAndColorAndRenderQueueAndActive();

        if (forward)
        {
            sky.StartFade(false);
            buildingToSky.StartFade(true);
        }
        else
        {
            buildingToSky.StartFade(true);
            doorToBuilding.StartFade(false);
        }
    }
    private void OnWalkerProgress()
    {
        UpdateBuildingsMesh();
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
    private void SetBuildingsRoleIndexAndColorAndRenderQueueAndActive()
    {
        SetBuildingsRoleIndices();
        SetBuildingsColor();
        SetBuildingsRenderQueue();
        SetBuildingsActive();
    }
    private void SetBuildingsRoleIndices()
    {
        sky = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex);
        buildingToSky = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex + 1);
        doorToBuilding = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex + 2);
        zeroToDoor = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex + 3);
        unseen1 = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex + 4);
        unseen2 = Extensions.SafeGet<Building>(buildings, currSkyBldgIndex + 5);
    }
    private void SetBuildingsColor()
    {
        Color skyCol = Extensions.SafeGet<Color>(colors, currSkyColorIndex);
        Color buildingToSkyCol = Extensions.SafeGet<Color>(colors, currSkyColorIndex + 1);
        Color doorToBuildingCol = Extensions.SafeGet<Color>(colors, currSkyColorIndex + 2);
        Color zeroToDoorCol = Extensions.SafeGet<Color>(colors, currSkyColorIndex + 3);

        sky.SetColors(skyCol, buildingToSkyCol);
        buildingToSky.SetColors(buildingToSkyCol, doorToBuildingCol);
        doorToBuilding.SetColors(doorToBuildingCol, zeroToDoorCol);
        zeroToDoor.SetColors(zeroToDoorCol, Color.black);
    }
    private void SetBuildingsRenderQueue()
    {
        sky.SetRenderQueue(renderQueueSky);
        buildingToSky.SetRenderQueue(renderQueuebuildingToSky);
        doorToBuilding.SetRenderQueue(renderQueuedoorToBuilding);
        zeroToDoor.SetRenderQueue(renderQueuezeroToDoor);
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

    #region set buildings Mesh (call every frame where user moves)
    private void UpdateBuildingsMesh()
    {
        sky.UpdateMesh(Walker.progress, 0);
        buildingToSky.UpdateMesh(Walker.progress, 1);
        doorToBuilding.UpdateMesh(Walker.progress, 2);
        zeroToDoor.UpdateMesh(Walker.progress, 3);
    }
    #endregion
}