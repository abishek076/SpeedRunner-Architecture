using UnityEngine;

public class RandomObstacle : MonoBehaviour
{
    [Header("Visual Models")]
    // An array to hold the different 3D models attached to this prefab
    public GameObject[] obstacleModels;

    // OnEnable is called the exact millisecond the Object Pooler pulls this from the queue and activates it
    void OnEnable()
    {
        // Safety check to prevent errors
        if (obstacleModels.Length == 0) return;

        // 1. Turn OFF every model inside the array
        foreach (GameObject model in obstacleModels)
        {
            model.SetActive(false);
        }

        // 2. Pick a random number between 0 and however many models you added
        int randomIndex = Random.Range(0, obstacleModels.Length);

        // 3. Turn ON only the randomly selected model
        obstacleModels[randomIndex].SetActive(true);
    }
}