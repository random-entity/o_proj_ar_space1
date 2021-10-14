using UnityEngine;

public class LandPathSystem : MonoBehaviour
{
    private LandPath landPath;

    private void Awake()
    {
        landPath = GetComponentInChildren<LandPath>();
    }

    private void OnWalkerProgress()
    {
        landPath.SetAngWid(Building.GetAngWid(Walker.Progress, 1), Building.GetAngWid(Walker.Progress, 2));
    }
    private void OnWalkerStageSwitch(bool forward)
    {
        landPath.SwapColors();
        landPath.StartFade(forward);
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
}
