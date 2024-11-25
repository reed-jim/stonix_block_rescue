using UnityEngine;

public class ScrewPuzzleInputManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerMaskCheck;

    private IScrew _selectedScrew;
    private IScrewPort _selectedScrewPort;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMaskCheck);

            if (_selectedScrew == null)
            {
                if (hit.collider != null)
                {
                    _selectedScrew = hit.collider.GetComponent<IScrew>();

                    if (_selectedScrew != null)
                    {
                        _selectedScrew.Select();

                        PlaySoundSelectScrew();
                    }

                    Debug.Log(hit.collider);
                    Debug.Log(_selectedScrew);
                }
            }
            else
            {
                if (hit.collider != null)
                {
                    _selectedScrewPort = hit.collider.GetComponent<IScrewPort>();

                    if (_selectedScrewPort != null)
                    {
                        _selectedScrew.BreakJoint();
                        _selectedScrewPort.PutScrew(_selectedScrew);

                        PlaySoundPutScrew();

                        _selectedScrew = null;
                        _selectedScrewPort = null;
                    }
                }

                // _selectedScrew.Move(mousePosition);
            }
        }
        // else
        // {
        //     _selectedScrew = null;
        // }
    }

    private void PlaySoundSelectScrew()
    {
        AudioSource selectSound = ObjectPoolingEverything.GetFromPool("SelectScrew").GetComponent<AudioSource>();

        selectSound.gameObject.SetActive(true);

        selectSound.Play();
    }

    private void PlaySoundPutScrew()
    {
        AudioSource selectSound = ObjectPoolingEverything.GetFromPool("PutScrew").GetComponent<AudioSource>();

        selectSound.gameObject.SetActive(true);

        selectSound.Play();
    }
}
