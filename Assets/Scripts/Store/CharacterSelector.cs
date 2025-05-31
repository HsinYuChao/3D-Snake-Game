using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空間

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] characters;        // 角色 Prefabs
    public string[] characterNames;        // 對應角色的名稱
    public TextMeshProUGUI nameText;       // 顯示名稱的 UI 元件

    private int currentIndex = 0;
    private GameObject currentCharacter;
    private GameObject currentContainer;

    void Start()
    {
        ShowCharacter(currentIndex);
    }

    public void ShowCharacter(int index)
    {
        // 刪掉舊的角色容器
        if (currentContainer != null)
        {
            Destroy(currentContainer);
        }

        // 新建一個角色容器
        currentContainer = new GameObject("CharacterContainer");
        currentContainer.transform.SetParent(transform, false);
        currentContainer.transform.localRotation = Quaternion.Euler(0, 180, 0);

        // 建立新角色並加到容器下
        currentCharacter = Instantiate(characters[index], currentContainer.transform);
        currentCharacter.transform.localScale = Vector3.one * 7f;

        // 加入旋轉腳本
        currentContainer.AddComponent<RotateCharacter>();

        // 顯示對應的角色名稱
        if (nameText != null && index < characterNames.Length)
        {
            nameText.text = characterNames[index];
        }
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void SelectCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}


