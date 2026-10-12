using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 3f; 
    public float sideSpeed = 10f; 
    private int desiredLane = 1; 

    [Header("Nitro & Brake")]
    public float nitroZPosition = 3.5f; 
    public float brakeZPosition = -2.5f; 
    private float defaultZPosition;
    private float targetZ;

    [Header("Cinematics & Audio")]
    public CinemachineCamera playerCam; 
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    public float nitroShakeIntensity = 2.5f;

    public AudioSource engineSource;      
    public AudioSource effectSource;      
    public AudioClip revSound;            
    public AudioClip brakeSound;          
    public AudioClip crashSound;          

    private bool isRevving = false;
    private bool isBraking = false;
    
    // NEW: Timer to pause the engine hum
    private float revTimer = 0f;

    [Header("Crash Effects")]
    public ParticleSystem crashSparks;

    void Start()
    {
        defaultZPosition = transform.position.z;
        targetZ = defaultZPosition;
        
        if (playerCam != null)
        {
            cameraNoise = playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // 1. MENU LOGIC: If the game is on the Main Menu or Game Over screen, kill the audio and freeze the car
        if (GameManager.isGameOver)
        {
            if (engineSource != null && engineSource.isPlaying) engineSource.Stop();
            return; 
        }

        // 2. THE 1-SECOND REV PAUSE LOGIC
        if (revTimer > 0f)
        {
            revTimer -= Time.deltaTime; // Count down the timer
        }
        else if (engineSource != null && !engineSource.isPlaying)
        {
            // The 1 second is up, turn the continuous engine hum back on!
            engineSource.Play();
        }

        HandleLaneInput();
        HandleSpeedInput();
        MoveCar();
    }

    private void HandleLaneInput()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2; 
        }
        
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0; 
        }
    }

    private void HandleSpeedInput()
    {
        // Nitro Boost (W / Up Arrow)
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            targetZ = nitroZPosition;
            if (cameraNoise != null) cameraNoise.AmplitudeGain = nitroShakeIntensity;
            
            // Fire the Rev sound and Pause the engine for exactly 1 second
            if (!isRevving)
            {
                if (effectSource != null && revSound != null) effectSource.PlayOneShot(revSound);
                if (engineSource != null) engineSource.Pause();
                
                revTimer = 1f; // Set the pause timer to 1 second
                isRevving = true;
            }

            // Only pitch the engine up if the 1-second pause is finished and it is playing again
            if (engineSource != null && engineSource.isPlaying) engineSource.pitch = 1.5f; 
        }
        // Emergency Brake (S / Down Arrow)
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            targetZ = brakeZPosition;
            if (cameraNoise != null) cameraNoise.AmplitudeGain = 0f;
            
            if (engineSource != null && engineSource.isPlaying) engineSource.pitch = 0.7f;
            
            if (!isBraking && effectSource != null && brakeSound != null)
            {
                effectSource.PlayOneShot(brakeSound);
                isBraking = true;
            }
        }
        // Coasting (No Keys Pressed)
        else
        {
            targetZ = defaultZPosition;
            if (cameraNoise != null) cameraNoise.AmplitudeGain = 0f;
            if (engineSource != null && engineSource.isPlaying) engineSource.pitch = 1.0f;
            
            isRevving = false;
            isBraking = false;
        }
    }

    private void MoveCar()
    {
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        if (desiredLane == 2) targetX = laneDistance;

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, sideSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (effectSource != null && crashSound != null) effectSource.PlayOneShot(crashSound);
            if (engineSource != null) engineSource.Stop();

            if (crashSparks != null) crashSparks.Play();

            FindAnyObjectByType<GameManager>().TriggerGameOver();
            this.enabled = false; 
        }
    }
}