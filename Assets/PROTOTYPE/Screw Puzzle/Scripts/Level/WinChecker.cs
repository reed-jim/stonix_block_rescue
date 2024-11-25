using System;
using System.Collections.Generic;
using UnityEngine;

public class WinChecker : MonoBehaviour
{
    [SerializeField] private int requiredNumberOfWood;

    private List<int> _checkedWoods;
    private int _currentNumberOfWood;
    private bool _isWon;

    public static event Action winLevelEvent;

    private void Awake()
    {
        _checkedWoods = new List<int>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isWon)
        {
            return;
        }

        if (other.GetComponent<IScrewPort>() != null)
        {
            return;
        }

        if (_checkedWoods.Contains(other.gameObject.GetInstanceID()))
        {
            return;
        }
        else
        {
            _checkedWoods.Add(other.gameObject.GetInstanceID());
        }

        _currentNumberOfWood++;

        if (_currentNumberOfWood >= requiredNumberOfWood)
        {
            winLevelEvent?.Invoke();

            _isWon = true;
        }
    }
}
