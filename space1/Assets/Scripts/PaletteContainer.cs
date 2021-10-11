using UnityEngine;

[System.Serializable]
public struct Palette
{
    public Color[] colors;
}

public class PaletteContainer : MonoSingleton<PaletteContainer>
{
    public Palette palette;
}