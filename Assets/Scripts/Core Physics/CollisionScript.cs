using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    public GameObject particleSystem;
    public AudioClip soundEffect;

    private bool hasCollided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain") && !hasCollided)
        {
            hasCollided = true;
            Instantiate(particleSystem, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(soundEffect, transform.position);
            Destroy(gameObject);
        }
    }
}
