using System;
using UnityEngine;


public class BoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_closedBox;
    [SerializeField] private GameObject m_openedBox;


    private void Start()
    {
        CloseBox();
    }


    [ContextMenu(nameof(OpenBox))]
    public void OpenBox()
    {
        m_closedBox.SetActive(false);
        m_openedBox.SetActive(true);
    }

    [ContextMenu(nameof(CloseBox))]
    public void CloseBox()
    {
        m_closedBox.SetActive(true);
        m_openedBox.SetActive(false);
    }
}