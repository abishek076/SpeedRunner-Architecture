SpeedRunner | 2.5D Arcade Racing Engine
(Note: Record a 5 to 10-second GIF of your car boosting, changing lanes, and crashing, and replace this link with the uploaded image file).

Overview
SpeedRunner is a high-performance 2.5D arcade racing game engineered in Unity using C#. It was architected specifically to maintain a flawless 60 FPS on hardware-constrained systems (8GB RAM) by bypassing CPU-heavy physics calculations in favor of mathematical kinematics and strict memory management[cite: 10].

This project serves as a technical showcase of custom character controllers, enterprise-level object pooling, and dynamic state management.

Technical Stack
Engine: Unity 2022 LTS (Universal Render Pipeline)

Language: C#

Architecture: Component-Based Entity Design, Object Pooling, State Machines

Packages: Cinemachine 3.x, Unity Input System

Core System Architecture & Documentation
1. The Kinematic Treadmill (Illusion of Speed)
Standard racing games push the player forward through a static world, which eventually leads to floating-point precision errors on the Z-axis. To prevent this, SpeedRunner utilizes a Treadmill Architecture.

Programmed math-driven Z-axis manipulation to create a seamless illusion of speed without relying on heavy raw physics forces[cite: 10].

The Player GameObject remains locked at Z = 0 while a custom WorldScroller.cs script pulls the ground and environmental hazards backward.

Once a ground segment passes behind the camera's frustum, it seamlessly teleports back to the horizon, creating an infinite, seamless loop.

2. Math-Driven Lane Controller
Rather than relying on Unity's Rigidbody.AddForce (which can feel floaty and unpredictable), the vehicle's movement is strictly controlled by linear mathematics.

Engineered a highly optimized 3-lane vehicle controller using Vector3.Lerp[cite: 10].

This allows for snappy, arcade-style lane switching that remains perfectly responsive regardless of frame rate drops.

Integrated Unity's modern Input System for responsive keyboard controls and utilized Cinemachine 3.x to trigger dynamic camera shakes during nitro boosts[cite: 10].

3. Enterprise Object Pooling (Memory Management)
To prevent the game from crashing on low-end hardware, dynamic memory allocation (Instantiate and Destroy) was completely removed from the runtime loop.

Architected an enterprise-level Object Pooling system to dynamically recycle low-poly 3D roadblocks and hazards[cite: 10].

By instantiating a fixed pool of barricades at startup and simply toggling SetActive() as they cycle across the screen, the system eliminated garbage collection hitches and memory leaks, maintaining a flawless 60 FPS on constrained 8GB RAM hardware[cite: 10].

4. Game State & Audio Management
The game logic is fully decoupled from the physics execution, managed by a centralized GameManager utilizing Unity's Time.timeScale.

Developed a centralized GameManager script to track player distance mathematically via Time.deltaTime[cite: 10].

Implemented a dynamic difficulty progression system that automatically scales game speed multipliers upon reaching specific distance thresholds[cite: 10].

Programmed a Trigger-based collision system featuring Invincibility Frames (I-Frames) via C# Coroutines[cite: 10].

Paired mechanics with a layered audio system utilizing dynamic pitch-shifting for engine hums and metallic crash impacts[cite: 10].

Core Code Implementation
PlayerMovement.cs (Kinematics & Audio)
Handles the lane-switching mathematics, nitro boost logic, and dynamic audio pitching.

C#
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 3f; 
    public float sideSpeed = 10f; 
    private int desiredLane = 1; 
    private float targetZ;

    [Header("Audio")]
    public AudioSource engineSource;      
    public AudioSource effectSource;      
    public AudioClip revSound;            
    public AudioClip crashSound;          
    private float revTimer = 0f;

    void Update()
    {
        if (GameManager.isGameOver)
        {
            if (engineSource != null && engineSource.isPlaying) engineSource.Stop();
            return; 
        }

        HandleTimers();
        HandleLaneInput();
        HandleSpeedInput();
        MoveCar();
    }

    private void MoveCar()
    {
        float targetX = (desiredLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, sideSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (effectSource != null) effectSource.PlayOneShot(crashSound);
            FindAnyObjectByType<GameManager>().TriggerGameOver();
            this.enabled = false; 
        }
    }
}
GameManager.cs (State Control)
Manages the transition between the Main Menu, Gameplay loop, and the Retry state by freezing the physics engine (Time.timeScale).

C#
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI distanceText;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;

    public static bool isGameOver = true; 
    private float distance;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        distanceText.gameObject.SetActive(false);
        Time.timeScale = 0f; // Freeze background physics
    }

    void Update()
    {
        if (isGameOver) return; 

        // Frame-rate independent distance tracking
        distance += 10f * Time.deltaTime;
        distanceText.text = Mathf.FloorToInt(distance).ToString() + "m";
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        distanceText.gameObject.SetActive(true);
        isGameOver = false;
        Time.timeScale = 1f; // Unfreeze physics
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Lock background to prevent repeated collisions
    }
}
Playable Build
You can play the compiled WebGL version of this engine directly in your browser here: [https://abishek13.itch.io/speed-runner]