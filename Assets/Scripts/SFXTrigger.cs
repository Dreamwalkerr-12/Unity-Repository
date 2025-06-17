using UnityEngine;

public class SFXTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sfxClip;
    public bool destroyOnPlay = true;

    [Header("Tag Filter")]
    public string triggerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (sfxClip != null)
                AudioManager.Instance.PlaySFX(sfxClip);

            if (destroyOnPlay)
                Destroy(gameObject);
        }
    }
}
