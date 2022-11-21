using UnityEngine;


public class ChangeMarbleToThisTransform : MonoBehaviour
{
    private ChangeMarblePosition _changeMarblePosition;
    private Transform _thisTransform;


    private void Awake()
    {
        _changeMarblePosition = FindObjectOfType<ChangeMarblePosition>();
        
        _thisTransform = transform;
    }


    /// <summary>
    ///     Call via UI button
    /// </summary>
    public void SetMarbleTransform()
    {
        _changeMarblePosition.PutMarbleHere(_thisTransform);
    }
}