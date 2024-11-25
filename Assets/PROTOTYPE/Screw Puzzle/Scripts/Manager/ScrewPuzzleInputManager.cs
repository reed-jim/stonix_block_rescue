using UnityEngine;

public class ScrewPuzzleInputManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerMaskCheck;

    private IScrew _selectedScrew;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_selectedScrew == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMaskCheck);

                if (hit.collider != null)
                {
                    _selectedScrew = hit.collider.GetComponent<IScrew>();
                    
                    _selectedScrew.BreakJoint();
                }
            }
            else
            {
                _selectedScrew.Move(mousePosition);
            }
        }
        else
        {
            _selectedScrew = null;
        }
    }
}
