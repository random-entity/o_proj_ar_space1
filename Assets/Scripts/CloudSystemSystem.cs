using UnityEngine;

public class CloudSystemSystem : MonoBehaviour
{
    #region fields
    [SerializeField] private CloudSystem cloudSystemPrefab;
    private CloudSystem[] cloudSystems;
    private CloudSystem sky, buildingToSky;
    int total = 2;
    int curr = 0;
    #endregion

    private void Awake()
    {
        Extensions.InitializeArray<CloudSystem>(out cloudSystems, cloudSystemPrefab, total, transform);
        setRole();

        sky.SetAlpha(1f);
        buildingToSky.SetAlpha(0f);
    }

    private void setRole()
    {
        sky = Extensions.SafeGet<CloudSystem>(cloudSystems, curr);
        buildingToSky = Extensions.SafeGet<CloudSystem>(cloudSystems, curr + 1);
    }

    #region event handling
    private void OnWalkerStageSwitch(bool forward)
    {
        Extensions.SafeModuloAdd(ref curr, 1, total);

        setRole();

        if (forward)
        {
            sky.SetRenderQueue(BuildingSystem.renderQueueSky + 1); // role 바꾼 후, 새로운 sky
            sky.StartFade(true);

            buildingToSky.SetRenderQueue(BuildingSystem.renderQueueSky - 1); // 전 sky, 완전 안 보이게
            buildingToSky.StartFade(false);
        }
        else
        {
            sky.SetRenderQueue(BuildingSystem.renderQueueSky + 1); // role 바꾼 후, 열고 나오는 sky
            sky.StartFade(true);

            buildingToSky.SetRenderQueue(BuildingSystem.renderQueuebuildingToSky + 1); // role 바꾼 후, 원래 sky였다가 사라져야 하는
            buildingToSky.StartFade(false);
        }
    }
    private void OnEnable()
    {
        Walker.StageSwitchAlert += OnWalkerStageSwitch;
    }
    private void OnDisable()
    {
        Walker.StageSwitchAlert -= OnWalkerStageSwitch;
    }
    #endregion
}