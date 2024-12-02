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

            RegisterMoreEvent();

            _transitionAnimation = GetComponent<ISaferioUIAnimation>();

            MoreActionInAwake();
        }

        private void OnDestroy()
        {
            SwitchRouteButton.switchRouteEvent -= OnRouteSwitched;

            UnregisterMoreEvent();
        }

        protected virtual void RegisterMoreEvent()
        {

        }

        protected virtual void UnregisterMoreEvent()
        {

        }

        protected virtual void MoreActionInAwake()
        {

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

        protected void Show()
        {
            _transitionAnimation.Show();
        }

        protected void Hide()
        {
            _transitionAnimation.Hide();
        }
    }
}
