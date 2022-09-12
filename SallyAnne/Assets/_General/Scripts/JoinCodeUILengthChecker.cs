using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JoinCodeUILengthChecker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_joinCodeInput;
    [SerializeField] private Button m_button;
    [SerializeField] private int m_joinCodeLength = 6;

    private string _tempJoinCode;


    private void Update()
    {
        CheckLength();
    }


    private void CheckLength()
    {
        if (m_joinCodeInput.text == _tempJoinCode)
        {
            return;
        }

        if (m_joinCodeInput.text.Length == m_joinCodeLength + 1)
        {
            m_button.interactable = true;
        }
        else
        {
            m_button.interactable = false;
        }

        _tempJoinCode = m_joinCodeInput.text;
    }
}