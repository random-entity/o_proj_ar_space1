using UnityEngine;

public class Walker : MonoBehaviour
{
    #region event broadcasters
    public delegate void VoidAlert();
    public static event VoidAlert ProgressAlert;
    public delegate void BoolAlert(bool forward);
    public static event BoolAlert StageSwitchAlert;
    #endregion

    public static float progress = 0f;
    private static float speed = 0.25f;

    private void walk(bool forward)
    {
        progress += (forward ? 1f : -1f) * speed * Time.deltaTime;

        if (progress >= 1f || progress < 0f)
        {
            if (progress >= 1f)
            {
                progress %= 1f;
                StageSwitchAlert(true);
            }
            else // if (playerWalk < 0f)
            {
                progress += 1f;
                StageSwitchAlert(false);
            }
        }
    }

    private void walk(float input)
    {
        if (input == 0) return;

        progress += input * speed * Time.deltaTime;

        if (progress >= 1f)
        {
            progress -= 1f;
            StageSwitchAlert(true);
        }
        else if (progress < 0f)
        {
            progress += 1f;
            StageSwitchAlert(false);
        }

        ProgressAlert();
    }

    void Update()
    {
        walk(Input.GetAxis("Vertical"));

        // if (Input.GetKey(KeyCode.W))
        // {
        //     walk(true);
        //     ProgressAlert();
        // }
        // else if (Input.GetKey(KeyCode.S))
        // {
        //     walk(false);
        //     ProgressAlert();
        // }
    }
}