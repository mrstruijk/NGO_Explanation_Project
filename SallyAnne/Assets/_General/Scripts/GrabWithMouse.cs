using UnityEngine;


public class GrabWithMouse : MonoBehaviour
{
    [Tooltip("Grabs Main Camera in case it's left empty")]
    [SerializeField] private Camera m_Camera;

    private bool _isDragging;

    private Rigidbody _rigidbody;

    private float _startXPos;
    private float _startYPos;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
    }


    private void Update()
    {
        if (_isDragging)
        {
            DragObject();
        }
    }


    private void OnMouseDown()
    {
        var mousePos = Input.mousePosition;

        if (!m_Camera.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = m_Camera.ScreenToWorldPoint(mousePos);

        var localPos = transform.localPosition;

        _startXPos = mousePos.x - localPos.x;
        _startYPos = mousePos.y - localPos.y;

        _isDragging = true;
    }


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
        transform.localPosition = new Vector3(mousePos.x - _startXPos, mousePos.y - _startYPos, transform.localPosition.z);

        _rigidbody.velocity = new Vector3(0, 0, 0);
    }
}