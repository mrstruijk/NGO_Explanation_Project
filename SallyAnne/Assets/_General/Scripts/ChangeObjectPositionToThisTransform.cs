using UnityEngine;


public class ChangeObjectPositionToThisTransform : MonoBehaviour
{
    private ChangeThisObjectPositionViaServer _changeObjectPosition;


    private void Awake()
    {
        _changeObjectPosition = FindObjectOfType<ChangeThisObjectPositionViaServer>();
    }


    /// <summary>
    ///     Call via UI button
    /// </summary>
    public void SetObjectToThisTransform()
    {
        _changeObjectPosition.PutObjectHere(transform.position);
    }
}