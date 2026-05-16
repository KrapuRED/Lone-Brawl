using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null && audioSource.clip != null)
        {
            Destroy(gameObject, audioSource.clip.length);
        }
        else
        {
            Destroy(gameObject, 2f); 
        }
    }
}