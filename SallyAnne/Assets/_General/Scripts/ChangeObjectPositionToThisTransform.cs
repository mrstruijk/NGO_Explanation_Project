using UnityEngine;


/// <summary>
///     This calls the ServerAuthorityChangePosition script to put the object in that script to the position of the target
///     in this script.
/// </summary>
public class ChangeObjectPositionToThisTransform : MonoBehaviour
{
    [Header("Set in the Inspector or this transform will be used")]
    [SerializeField] private Transform m_target;
    
    private ServerAuthorityChangePosition _changeObjectPosition;


    private void Awake()
    {
        if (m_target == null)
        {
            m_target = transform;
        }

        _changeObjectPosition = FindObjectOfType<ServerAuthorityChangePosition>();
    }


    /// <summary>
    ///     Call via UI button
    /// </summary>
    public void SetObjectToThisTransform()
    {
        _changeObjectPosition.PutObjectHere(m_target.position);
    }
}