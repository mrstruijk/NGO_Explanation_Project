using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Currentlyh this needs to be on the main object since MouseDown and MouseUp rely on for instance the collider, i believe
/// 
/// </summary>
public class MoveWithMouse : NetworkBehaviour
{
    [Tooltip("Grabs Main Camera in case it's left empty")]
    [SerializeField] private Camera m_Camera;

    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private OwnershipManager m_ownershipManager;
    [SerializeField] private ChangeThisObjectPositionViaServer m_positionChanger;
    [SerializeField] private Transform m_objectToMove;
    private bool _isDragging;

    private float _startXPos;
    private float _startYPos;


    private void Awake()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        if (m_rigidbody == null)
        {
            m_rigidbody = GetComponentInParent<Rigidbody>();
        }

        if (m_ownershipManager == null)
        {
            m_ownershipManager = FindObjectOfType<OwnershipManager>();
        }

        if (m_positionChanger == null)
        {
            m_positionChanger = FindObjectOfType<ChangeThisObjectPositionViaServer>();
        }
    }


    private void Update()
    {
        if (_isDragging)
        {
            DragObject();
        }
    }

//TODO: CHange this to be able to operate on child objet
    private void OnMouseDown()
    {
        var mousePos = Input.mousePosition;

        if (!m_Camera.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = m_Camera.ScreenToWorldPoint(mousePos);

        var localPos = m_objectToMove.localPosition;

        _startXPos = mousePos.x - localPos.x;
        _startYPos = mousePos.y - localPos.y;

        _isDragging = true;

        Debug.LogFormat("X={0}, Y={1}", _startXPos, _startYPos);
    }

    
//TODO: CHange this to be able to operate on child objet
    private void OnMouseUp()
    {
        _isDragging = false;
    }


    private void DragObject()
    {
        var mousePos = Input.mousePosition;

        if (!m_Camera.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = m_Camera.ScreenToWorldPoint(mousePos);
        var newPos = new Vector3(mousePos.x - _startXPos, mousePos.y - _startYPos, m_objectToMove.localPosition.z);
        
        m_positionChanger.PutObjectHere(newPos);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPosServerRpc(Vector3 newPos)
    {
        m_objectToMove.localPosition = newPos;
        m_rigidbody.velocity = new Vector3(0, 0, 0);
        Debug.LogFormat("We dragged, localPos is now {0}", newPos);
    }
}