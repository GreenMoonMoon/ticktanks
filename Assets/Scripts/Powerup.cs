using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public Transform mesh;
    public float rotationSpeed, floatingFrequency;
    public string powerUpType;

    void Update()
    {
        mesh.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        IPowerable powerable = collider.GetComponent<IPowerable>();
        if (powerable != null)
        {
            powerable.Powerup(powerUpType);
            Destroy(gameObject);
        }
    }
}
