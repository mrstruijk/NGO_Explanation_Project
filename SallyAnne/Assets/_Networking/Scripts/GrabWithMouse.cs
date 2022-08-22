using UnityEngine;


public class GrabWithMouse : MonoBehaviour
{
    public Camera myCam;

    private bool _isDragging;

    private float _startXPos;
    private float _startYPos;


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

        if (!myCam.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = myCam.ScreenToWorldPoint(mousePos);

        _startXPos = mousePos.x - transform.localPosition.x;
        _startYPos = mousePos.y - transform.localPosition.y;

        _isDragging = true;
    }


    private void OnMouseUp()
    {
        _isDragging = false;
    }


    public void DragObject()
    {
        var mousePos = Input.mousePosition;

        if (!myCam.orthographic)
        {
            mousePos.z = 10;
        }

        mousePos = myCam.ScreenToWorldPoint(mousePos);
        transform.localPosition = new Vector3(mousePos.x - _startXPos, mousePos.y - _startYPos, transform.localPosition.z);
    }
}