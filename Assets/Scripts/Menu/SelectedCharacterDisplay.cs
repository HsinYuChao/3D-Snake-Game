using UnityEngine;

public class SelectedCharacterDisplay : MonoBehaviour
{
    public GameObject[] characters; // �� CharacterSelector �@�˪����� prefab �}�C

    private GameObject currentCharacter;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);

        if (selectedIndex >= 0 && selectedIndex < characters.Length)
        {
            // ��ܤw��ܪ�����
            currentCharacter = Instantiate(characters[selectedIndex], transform);
            currentCharacter.transform.localScale = Vector3.one * 7f;
            currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0);

            currentCharacter.AddComponent<RotateCharacter>(); // �[�J����ʵe

        }
    }
}
