using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionBinder : MonoBehaviour
{
    [Serializable]
    public class Binding
    {
        public string name;
        public InputActionReference actionReference;
        public UnityEvent @event;
        public bool continues = false;
    }
        
    [SerializeField] private List<Binding> actionBindings;

    private void Start()
    {
        foreach (var binding in actionBindings)
        {
            var action = binding.actionReference.action;
            if (!binding.continues)
            {
                action.performed += _ => binding.@event.Invoke();
            }
                
            action.Enable();
            action.actionMap.Enable();
            binding.actionReference.asset.Enable();
        }
    }

    private void Update()
    {
        foreach (var binding in actionBindings)
        {
            if (binding.continues)
            {
                if (binding.actionReference.action.IsPressed())
                {
                    binding.@event.Invoke();
                }
            }
        }
    }
}