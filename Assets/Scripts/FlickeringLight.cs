using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Flicker Pattern")]
    public int minFlickers = 3;
    public int maxFlickers = 5;
    public float flickerSpeed = 0.1f;

    [Header("Pause Between Flickers")]
    public float minPauseDuration = 2f;
    public float maxPauseDuration = 5f;

    [Header("Intensity Settings")]
    public float normalIntensity = 1f;
    public float darkIntensity = 0f;

    private Light lightComponent;
    private float timer;
    private int flickersRemaining;
    private bool isPaused;
    private float pauseTimer;

    void Start()
    {
        lightComponent = GetComponent<Light>();
        normalIntensity = lightComponent.intensity;
        StartNewFlickerSequence();
    }

    void Update()
    {
        if (isPaused)
        {
            lightComponent.intensity = darkIntensity;
            pauseTimer -= Time.deltaTime;

            if (pauseTimer <= 0)
            {
                isPaused = false;
                StartNewFlickerSequence();
            }
        }
        else
        {
            // Flickering phase - fix duration for next iteration and add mistt for more spookiness
            timer += Time.deltaTime;

            if (timer >= flickerSpeed)
            {
                lightComponent.intensity = Random.Range(0.5f, normalIntensity);
                timer = 0;
                flickersRemaining--;

                if (flickersRemaining <= 0)
                {
                    isPaused = true;
                    pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
                    lightComponent.intensity = darkIntensity; 
                }
            }
        }
    }

    void StartNewFlickerSequence()
    {
        flickersRemaining = Random.Range(minFlickers, maxFlickers + 1);
        timer = 0;
    }
}
