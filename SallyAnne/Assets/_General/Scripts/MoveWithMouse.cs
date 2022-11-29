using Unity.Netcode;
using UnityEngine;


/// <summary>
///     This class, which implements IRespondToMouse, needs to be in a child gameobject of a MouseHandler script.
///     If the left mouse button is pressed, it will calculate where the object (from the ServerAuthorityChangePosition
///     class) should be moved to, and upon release of the button, it will stop making this calculation.
/// </summary>
public class MoveWithMouse : NetworkBehaviour, IRespondToMouse
{
    [Header("Set in the Inspector or will be found")]
    [Tooltip("Finds Main Camera in case it's left empty")]
    [SerializeField] private Camera m_Camera;
    [Tooltip("Finds this component in case it's left empty")]
    [SerializeField] private ServerAuthorityChangePosition m_positionChanger;
    private bool _isDragging;

    private float _startXPos;
    private float _startYPos;


    /// <summary>
    ///     Upon mouse click, this calculates the starting location of the object.
    /// </summary>
    public void OnMouseButtonDown()
    {
        if (!m_positionChanger.ObjectToMovePresent())
        {
            return;
        }

        var mousePos = Input.mousePosition;

        if (!m_Camera.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = m_Camera.ScreenToWorldPoint(mousePos);

        var localPos = m_positionChanger.ObjectToMove.localPosition;

        _startXPos = mousePos.x - localPos.x;
        _startYPos = mousePos.y - localPos.y;

        _isDragging = true;
    }


    /// <summary>
    ///     Stop dragging as soon as you de-click the mouse.
    /// </summary>
    public void OnMouseButtonUp()
    {
        _isDragging = false;
    }


    private void Awake()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        if (m_positionChanger == null)
        {
            m_positionChanger = FindObjectOfType<ServerAuthorityChangePosition>();
        }
    }


    private void Update()
    {
        if (_isDragging)
        {
            DragObject();
        }
    }


    /// <summary>
    ///     This calculates where the object should move to.
    ///     Currently it will run after mouse click down, and before mouse click up (declick), and runs every frame in Update.
    /// </summary>
    private void DragObject()
    {
        if (!m_positionChanger.ObjectToMovePresent())
        {
            return;
        }

        var mousePos = Input.mousePosition;

        if (!m_Camera.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = m_Camera.ScreenToWorldPoint(mousePos);

        var newPos = new Vector3(mousePos.x - _startXPos, mousePos.y - _startYPos, m_positionChanger.ObjectToMove.localPosition.z);

        NotifyPositionChanger(newPos);
    }


    private void NotifyPositionChanger(Vector3 newPosition)
    {
        m_positionChanger.PutObjectHere(newPosition);
    }
}