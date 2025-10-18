using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour
{
    //public GameObject OrbitalPoint;
    public Transform myParent;
    public Vector3 axis = Vector3.up;
    Vector3 desiredPosition;
    public float radius = 2.0f;
    float radiusSpeed = 0.5f;
    public float orbitalSpeed = 80.0f;

    void Start()
    {
        myParent = gameObject.transform.parent;
    }

    void Update()
    {
        transform.RotateAround(myParent.position, axis, orbitalSpeed * Time.deltaTime * -1);
        desiredPosition = (transform.position - myParent.position).normalized * radius + myParent.position;
        transform.position = (transform.position - myParent.position).normalized * radius + myParent.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}