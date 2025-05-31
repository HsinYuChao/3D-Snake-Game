using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotateCharacter : MonoBehaviour
{
    public float rotationSpeed = 60f;

    // 可以在 Inspector 設定哪些場景會轉
    public List<string> activeInScenes = new List<string> { "Menu", "Store" };

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 如果不是在指定場景，就停用這個腳本
        if (!activeInScenes.Contains(currentScene))
        {
            enabled = false;
        }
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}