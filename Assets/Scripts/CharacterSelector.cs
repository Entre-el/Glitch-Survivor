using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance;
    public CharacterSO charaterData;
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static CharacterSO GetData()
    {
        return Instance.charaterData;
    }
    public void SelectCharater(CharacterSO character)
    {
        charaterData = character;
    }
    public void DestroySingleton()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
