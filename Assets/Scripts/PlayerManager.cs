using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterBundle
{
    public GameObject playerPrefab;
    public GameObject bodyPrefab;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public CharacterBundle[] characters;
    public int defaultIndex = 0;

    [SerializeField]
    private List<string> gameplaySceneNames = new List<string> { };

    public GameObject spawnedPlayer { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameplaySceneNames.Contains(scene.name))
        {
            SpawnSelectedPlayer();
        }
    }

    private void SpawnSelectedPlayer()
    {
        if (spawnedPlayer != null)
        {
            Destroy(spawnedPlayer); // �T�O���|�ͦ����ƨ���
        }

        Transform spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint")?.transform;
        if (spawnPoint == null)
        {
            Debug.LogError("�䤣�� PlayerSpawnPoint�I");
            return;
        }

        int index = PlayerPrefs.GetInt("SelectedCharacterIndex", defaultIndex);
        if (index < 0 || index >= characters.Length)
        {
            index = defaultIndex;
        }

        CharacterBundle bundle = characters[index];
        if (bundle.playerPrefab == null)
        {
            Debug.LogError("Player Prefab ���šI");
            return;
        }

        spawnedPlayer = Instantiate(bundle.playerPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedPlayer.name = "Player";
    }

    public GameObject GetSelectedBodyPrefab()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacterIndex", defaultIndex);
        return characters[index].bodyPrefab;
    }
}




