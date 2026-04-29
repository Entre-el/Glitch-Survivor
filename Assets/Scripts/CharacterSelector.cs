using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance;
    public CharacterBaseStatsSO charaterData;

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

    public static CharacterBaseStatsSO GetData()
    {
        return Instance.charaterData;
    }

    public void SelectCharater(CharacterBaseStatsSO character)
    {
        charaterData = character;
    }

    public void DestroySingleton()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
