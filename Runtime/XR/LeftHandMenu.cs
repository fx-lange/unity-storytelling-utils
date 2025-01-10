using UnityEngine;

namespace StorytellingUtils.XR
{
    public class LeftHandMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _menu;
        [SerializeField] private int _activationPressCount = 4;
        [SerializeField] private float _maxPressInterval = 2f;
        
        private bool _activated = false;
        private float _lastPress;
        private int _pressCount = 0;
        
        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (_activated)
                {
                    Debug.Log("START MENU");
                    _menu.SetActive(!_menu.activeSelf);
                }
                
                _lastPress = Time.time;
                _pressCount++;
            }

            if (_pressCount >= _activationPressCount)
            {
                _activated = !_activated;
                _pressCount = 0;
                
                _menu.SetActive(_activated);
            }

            if (Time.time - _lastPress > _maxPressInterval)
            {
                _pressCount = 0;
            }
        }
    }
}