using UnityEngine;

public class Shell : MonoBehaviour
{
    public float lifeSpan;
    public float explosionRadius;
    public Collider safeCollider;

    private void Awake()
    {
        Destroy(gameObject, lifeSpan);
    }

    void OnCollisionEnter(Collision collision)
    {
        DebugExtension.DebugCircle(transform.position, Color.red, explosionRadius, 1);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == safeCollider)
                continue;
            if(colliders[i].GetComponent<IDamagable>() != null)
            {
                Debug.Log(colliders[i]);
                colliders[i].GetComponent<IDamagable>().Damage();
            }
        }
        
        Destroy(gameObject);
    }
}
