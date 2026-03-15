using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterScriptableObject charaterData;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static CharacterScriptableObject GetData()
    {
        return instance.charaterData;
    }
    public void SelectCharater(CharacterScriptableObject character)
    {
        charaterData = character;
    }
    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
