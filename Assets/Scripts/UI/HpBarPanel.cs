using UnityEngine;
using UnityEngine.UI;
public class HpBarPanel : BasePanel 
{
    [SerializeField]
    private PlayerHealth playerHealth;
    public override void OnInit()
    {
        if(playerHealth == null)
        {
            if(!TryGetComponent<PlayerHealth>(out playerHealth))
            Debug.LogError("PlayerStats component not found");
            return;
        }
        base.OnInit();
    }
    public Slider hpSlider;
    public void Update()
    {
        if(playerHealth != null)
        {
            hpSlider.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;   
        }
    }
    
}
