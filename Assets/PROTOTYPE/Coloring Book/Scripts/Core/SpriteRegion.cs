using UnityEngine;

public class SpriteRegion : MonoBehaviour
{
    private Sprite _sprite;

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }
}
