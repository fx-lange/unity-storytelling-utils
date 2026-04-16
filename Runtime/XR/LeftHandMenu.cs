using UnityEngine;
using UnityEngine.Events;

namespace StorytellingUtils.XR
{
    public class LeftHandMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _menu;
        [SerializeField][Range(1,5)] private int _activationPressCount = 4;
        [SerializeField] private float _maxPressInterval = 2f;

        public UnityEvent onMenuOpened;
        public UnityEvent onMenuClosed;

        private float _lastPress;
        private int _pressCount = 0;

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (_menu.activeSelf)
                {
                    _menu.SetActive(false);
                    onMenuClosed?.Invoke();
                    return;
                }

                _lastPress = Time.time;
                _pressCount++;

                if (_pressCount >= _activationPressCount)
                {
                    _menu.SetActive(true);
                    onMenuOpened?.Invoke();
                    _pressCount = 0;
                }
            }

            if (Time.time - _lastPress > _maxPressInterval)
            {
                _pressCount = 0;
            }
        }
    }
}
