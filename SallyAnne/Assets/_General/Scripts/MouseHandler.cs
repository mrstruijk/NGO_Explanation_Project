using UnityEngine;


/// <summary>
///     MouseEvent handler which can be placed on any object with a Collider attached
///     Will find child components with IRespondToMouse interface attached.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MouseHandler : MonoBehaviour
{
    private IRespondToMouse[] _mouseResponders = { };


    private void Awake()
    {
        _mouseResponders = GetComponentsInChildren<IRespondToMouse>();
    }


    private void Start()
    {
        InterfacesFound();
    }


    private void OnMouseDown()
    {
        if (!InterfacesFound())
        {
            return;
        }

        foreach (var mouseResponder in _mouseResponders)
        {
            mouseResponder.OnMouseButtonDown();
        }
    }


    private void OnMouseUp()
    {
        if (!InterfacesFound())
        {
            return;
        }

        foreach (var mouseResponder in _mouseResponders)
        {
            mouseResponder.OnMouseButtonUp();
        }
    }


    private bool InterfacesFound()
    {
        if (_mouseResponders == null || _mouseResponders.Length == 0)
        {
            Debug.LogError("Didn't find any child components with IRespondToMouse interface attached");

            return false;
        }

        return true;
    }
}