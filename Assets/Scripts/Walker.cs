using UnityEngine;

public class Walker : MonoBehaviour
{
    #region event broadcasters
    public delegate void VoidAlert();
    public static event VoidAlert ProgressAlert;
    public delegate void BoolAlert(bool forward);
    public static event BoolAlert StageSwitchAlert;
    #endregion

    public static float Progress = 0f;
    private static float speed = 0.25f;

    private void walk(bool forward)
    {
        Progress += (forward ? 1f : -1f) * speed * Time.deltaTime;

        if (Progress >= 1f || Progress < 0f)
        {
            if (Progress >= 1f)
            {
                Progress %= 1f;
                StageSwitchAlert(true);
            }
            else // if (playerWalk < 0f)
            {
                Progress += 1f;
                StageSwitchAlert(false);
            }
        }
    }

    private void walk(float input)
    {
        if (input == 0) return;

        Progress += input * speed * Time.deltaTime;

        if (Progress >= 1f)
        {
            Progress -= 1f;
            StageSwitchAlert(true);
        }
        else if (Progress < 0f)
        {
            Progress += 1f;
            StageSwitchAlert(false);
        }

        ProgressAlert();
    }

    void Update()
    {
        walk(Input.GetAxis("Vertical"));
    }
}