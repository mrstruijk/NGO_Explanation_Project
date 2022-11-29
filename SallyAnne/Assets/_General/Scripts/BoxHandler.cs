using UnityEngine;


[RequireComponent(typeof(Collider))]
public class BoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_marble;
    [SerializeField] private string m_controllerTag = "XR_Controller";
    [SerializeField] private GameObject m_closedBox;
    [SerializeField] private GameObject m_openedBox;
    private bool _boxOpen;


    private void Start()
    {
        OpenBox();
    }


    [ContextMenu(nameof(OpenBox))]
    public void OpenBox()
    {
        m_closedBox.SetActive(false);
        m_openedBox.SetActive(true);
        _boxOpen = true;
    }


    [ContextMenu(nameof(CloseBox))]
    public void CloseBox()
    {
        m_closedBox.SetActive(true);
        m_openedBox.SetActive(false);
        _boxOpen = false;
    }


    private void ToggleBox()
    {
        if (_boxOpen)
        {
            CloseBox();
        }
        else
        {
            OpenBox();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == m_marble)
        {
            if (_boxOpen)
            {
                CloseBox();
            }
        }

        if (GameObject.FindWithTag(m_controllerTag) == other.gameObject)
        {
            ToggleBox();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == m_marble)
        {
            if (!_boxOpen)
            {
                OpenBox();
            }
        }
    }
}