using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Bounds targetBound;
    //Vector3 offset;

    public List<Transform> targets;
    public Camera cam;
    public float speed;
    public float minZoom;
    public float maxZoom;

    void Awake()
    {
        //offset = transform.position - targets[0].position;
        UpdateBound();
    }

    void LateUpdate()
    {
        UpdateBound();
        transform.position = Vector3.Lerp(transform.position, targetBound.center, Time.deltaTime * speed);
        cam.orthographicSize = Mathf.Clamp(Mathf.Max(targetBound.size.x, targetBound.size.z) * 0.5f, minZoom, maxZoom);
    }

    void UpdateBound()
    {
        targetBound = new Bounds(targets[0].position, new Vector3(1, 0, 1));
        for(int i = 1; i < targets.Count; i++)
        {
            targetBound.Encapsulate(targets[i].position);
        }
    }
}
