using UnityEngine;

public class BuildingSystem : MonoSingleton<BuildingSystem>
{
    #region fields for Buildings config
    private Building[] buildings;
    [SerializeField] private Building buildingPrefab;
    private Building sky, buildingToSky, doorToBuilding, zeroToDoor, unseen1, unseen2;
    private WindowGroup[] windowGroups;
    [SerializeField] private WindowGroup windowGroupPrefab;
    private bool currWinGrpBool = false;
    private int getCurrWinGrpIndex()
    {
        return currWinGrpBool ? 1 : 0;
    }
    private int toggleCurrWinGrpBool()
    {
        currWinGrpBool = !currWinGrpBool;
        return getCurrWinGrpIndex();
    }
    #endregion

    #region fields for Buildings drawing
    public static float centerDir = Mathf.PI * 0.5f; // 빌딩의 중심축이 xz 평면에서 어떤 argument(각) 갖는지.
    public static float[] presetAngWids;
    public static float angWidStepRatio = 16f;
    public static float raduisStepRatio = 1.1f;
    public static float angWidToHeight = 1.5f;
    #endregion

    #region fields for processing player control input
    private float playerSpeed = 0.3f;
    private float playerWalk = 0f; // 1f가 되면 한 페이즈 종료.
    private int currStep = 0;
    // private int totalStep = 3 * 2;
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

    private void initiateArray<T>(out T[] array, int size, T prefab) where T : MonoBehaviour
    {
        array = new T[size];
        // Debug.Log(array == null);
        for (int i = 0; i < size; i++)
        {
            T ith = Instantiate(prefab);
            array[i] = ith;
            ith.transform.SetParent(transform);
        }
    }
    private void Awake()
    {
        initiateArray<Building>(out buildings, 6, buildingPrefab);
        initiateArray<WindowGroup>(out windowGroups, 2, windowGroupPrefab);
        windowGroups[0].InitiateAllAlpha(1f);
        windowGroups[1].InitiateAllAlpha(0f);

        presetAngWids = new float[4];
        presetAngWids[0] = 2f * Mathf.PI;
        for (int i = 1; i < presetAngWids.Length; i++)
        {
            presetAngWids[i] = presetAngWids[i - 1] / angWidStepRatio;
        }

        palette = Palette.instance.palette;

        SetBuildingsRoleIndicesAndColor();
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

    private void SetBuildingsRoleIndicesAndColor()
    {
        sky = buildings[currStep];
        buildingToSky = buildings[(currStep + 1) % 6];
        doorToBuilding = buildings[(currStep + 2) % 6];
        zeroToDoor = buildings[(currStep + 3) % 6];
        unseen1 = buildings[(currStep + 4) % 6];
        unseen2 = buildings[(currStep + 5) % 6];

        sky.setMaterialColor(getColorFromDayPhase(currDayPhase));
        buildingToSky.setMaterialColor(getColorFromDayPhase(currDayPhase + 1));
        doorToBuilding.setMaterialColor(getColorFromDayPhase(currDayPhase + 2));
        zeroToDoor.setMaterialColor(getColorFromDayPhase(currDayPhase + 3));
        sky.gameObject.SetActive(true);
        buildingToSky.gameObject.SetActive(true);
        doorToBuilding.gameObject.SetActive(true);
        zeroToDoor.gameObject.SetActive(true);
        unseen1.gameObject.SetActive(false);
        unseen2.gameObject.SetActive(false);
    }
    private void WalkAndUpdate(bool forward)
    {
        playerWalk += (forward ? 1f : -1f) * playerSpeed * Time.deltaTime;
        if (playerWalk >= 1f || playerWalk < 0f)
        {
            if (playerWalk >= 1f)
            {
                playerWalk %= 1f;

                currStep++;
                currDayPhase++;

                windowGroups[getCurrWinGrpIndex()].StartFades(false);
                windowGroups[toggleCurrWinGrpBool()].StartFades(true);
            }
            else // if (playerWalk < 0f)
            {
                playerWalk += 1f;

                currStep += 5;
                currDayPhase += totalDayPhase - 1;
            }

            if (currStep >= 6)
            {
                currStep %= 6;
            }
            if (currDayPhase >= totalDayPhase)
            {
                currDayPhase %= totalDayPhase;
            }

            SetBuildingsRoleIndicesAndColor();
        }

        UpdateBuildingsMesh();
    }
    private void UpdateBuildingsMesh()
    {
        sky.UpdateMesh(playerWalk, 0);
        buildingToSky.UpdateMesh(playerWalk, 1);
        doorToBuilding.UpdateMesh(playerWalk, 2);
        zeroToDoor.UpdateMesh(playerWalk, 3);

        windowGroups[getCurrWinGrpIndex()].SetMeshes(buildingToSky, doorToBuilding);
    }
}