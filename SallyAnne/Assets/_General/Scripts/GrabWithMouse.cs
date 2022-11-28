using Unity.Netcode;
using UnityEngine;


public class GrabWithMouse : NetworkBehaviour
{
    [Tooltip("Grabs Main Camera in case it's left empty")]
    [SerializeField] private Camera m_Camera;

    private bool _isDragging;
    private ulong _networkClientID;
    private NetworkObject _networkObject;

    private Rigidbody _rigidbody;

    private float _startXPos;
    private float _startYPos;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _networkObject = GetComponent<NetworkObject>();

        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
    }


    public override void OnNetworkSpawn()
    {
        _networkClientID = NetworkManager.Singleton.LocalClientId;
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

        Debug.LogFormat("X={0}, Y={1}", _startXPos, _startYPos);
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
        var newPos = new Vector3(mousePos.x - _startXPos, mousePos.y - _startYPos, transform.localPosition.z);
        SetPosServerRpc(newPos);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPosServerRpc(Vector3 newPos)
    {
        transform.localPosition = newPos;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        Debug.LogFormat("We dragged, localPos is now {0}", newPos);
    }
}