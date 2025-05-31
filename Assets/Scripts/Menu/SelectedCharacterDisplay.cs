using UnityEngine;

public class SelectedCharacterDisplay : MonoBehaviour
{
    public GameObject[] characters; // 跟 CharacterSelector 一樣的角色 prefab 陣列

    private GameObject currentCharacter;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);

        if (selectedIndex >= 0 && selectedIndex < characters.Length)
        {
            // 顯示已選擇的角色
            currentCharacter = Instantiate(characters[selectedIndex], transform);
            currentCharacter.transform.localScale = Vector3.one * 7f;
            currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0);

            currentCharacter.AddComponent<RotateCharacter>(); // 加入旋轉動畫

        }
    }
}
