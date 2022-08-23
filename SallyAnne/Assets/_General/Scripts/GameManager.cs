using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_matchMakingUI;
    [SerializeField] private List<GameObject> m_gameObjects;
    [SerializeField] private bool m_toggleAtStart = true;


    private void Start()
    {
        if (!m_toggleAtStart)
        {
            return;
        }

        m_matchMakingUI.SetActive(true);
        ToggleGameObjects(false);
    }


    private void ToggleGameObjects(bool enable)
    {
        foreach (var go in m_gameObjects)
        {
            go.SetActive(enable);
        }
    }


    public void StartGame()
    {
        ToggleGameObjects(true);
    }
}