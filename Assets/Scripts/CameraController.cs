using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Bounds targetBound;
    Vector3 offset;

    public List<Transform> targets;
    public float speed;

    void Awake()
    {
        offset = transform.position - targets[0].position;
        targetBound = InitBound();
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targets[0].position + offset, Time.deltaTime * speed);
        //transform.position = Vector3.Lerp(transform.position, targetBound.center, Time.deltaTime);
        //Debug.Log(targetBound.center);
    }

    Bounds InitBound()
    {
        Bounds b = new Bounds(targets[0].position, new Vector3(1, 0, 1));
        for(int i = 1; i < targets.Count; i++)
        {
            targetBound.Encapsulate(targets[i].position);
        }
        return b;
        Debug.Log(b.ToString());
    }
}
