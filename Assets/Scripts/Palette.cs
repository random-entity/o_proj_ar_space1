using UnityEngine;

public class Palette : MonoSingleton<Palette>
{
    public Color[] palette;

    private void Start() {
        if(palette.Length != BuildingSystem.totalDayPhase) {
            Debug.Log("palette.Length != BuildingSystem.totalDayPhase");
        }
    }
}