using System;
using Unity.VisualScripting;
using UnityEngine;
using static Saferio.Prototype.ColoringBook.GameEnum;

namespace Saferio.Prototype.ColoringBook
{
    [Serializable]
    public class Scaleano : ISaferioUIAnimation
    {
        public void Hide()
        {
            throw new System.NotImplementedException();
        }

        public void Show()
        {
            throw new System.NotImplementedException();
        }
    }

    [Serializable]
    public class Scaleassno : ISaferioUIAnimation
    {
        public void Hide()
        {
            throw new System.NotImplementedException();
        }

        public void Show()
        {
            throw new System.NotImplementedException();
        }
    }

    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private ScreenRoute route;

        private ISaferioUIAnimation _transitionAnimation;

        private void Awake()
        {
            SwitchRouteButton.switchRouteEvent += OnRouteSwitched;

            _transitionAnimation = GetComponent<ISaferioUIAnimation>();

            Debug.Log(_transitionAnimation);
        }

        private void OnDestroy()
        {
            SwitchRouteButton.switchRouteEvent -= OnRouteSwitched;
        }

        private void OnRouteSwitched(ScreenRoute route)
        {
            if (route == this.route)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            _transitionAnimation.Show();
        }

        private void Hide()
        {
            _transitionAnimation.Hide();
        }
    }
}
