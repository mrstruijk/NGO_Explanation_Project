using UnityEngine;


public class PutMarble : MonoBehaviour
{
    public GameObject Marble;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = Marble.GetComponent<Rigidbody>();
    }


    public void PutMarbleHere()
    {
        _rigidbody.isKinematic = true;
        Marble.transform.position = transform.position;
        _rigidbody.isKinematic = false;
    }
}