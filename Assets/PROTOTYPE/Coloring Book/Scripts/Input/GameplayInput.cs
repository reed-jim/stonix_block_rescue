using UnityEngine;

public class GameplayInput : MonoBehaviour
{
    [SerializeField] private LayerMask layerMaskCheck;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMaskCheck);

            if (hit.collider != null)
            {
                SpriteRegion spriteRegion = hit.collider.GetComponent<SpriteRegion>();

                if (spriteRegion != null)
                {
                    spriteRegion.FillColor();
                }
            }
        }
    }
}
