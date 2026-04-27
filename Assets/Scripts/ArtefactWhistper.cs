using UnityEngine;

public class ArtefactWhistper : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource whisperSource;

    [Header("Distance Settings")]
    public float maxHearDistance = 20f;  
    public float minDistance = 2f;        
    public float maxVolume = 1f;          
    public float minVolume = 0f;         

    [Header("Player Reference")]
    public string playerTag = "Seeker";  

    private Transform player;

    void Start()
    {
  
        if (whisperSource == null)
        {
            whisperSource = GetComponent<AudioSource>();
        }

       
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (whisperSource != null)
        {
            whisperSource.volume = 0f;
        }
    }

    void Update()
    {
        if (player != null && whisperSource != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= maxHearDistance)
            {
                float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxHearDistance - minDistance));
                float volume = Mathf.Lerp(maxVolume, minVolume, normalizedDistance);
                whisperSource.volume = volume;
            }
            else
            {
                whisperSource.volume = 0f;
            }
        }
    }
}
