using UnityEngine;

[RequireComponent(typeof(PlayerLocomotion))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerExperience))]
[RequireComponent(typeof(PlayerVisuals))] // 假设你加上了表现层
public class PlayerCore : MonoBehaviour
{
    [field: SerializeField] public TransformAnchorSO playerTransformAnchor { get; private set; } 
    // 假设你听了我的建议，创建了基础数据的 SO 并在面板拖拽赋值
    [SerializeField] private CharacterBaseStatsSO baseStatsSO;

    public PlayerLocomotion Locomotion { get; private set;}
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerExperience Experience { get; private set; }
    public PlayerVisuals Visuals { get; private set; }
    
    public StatCalculator Stats { get; private set; }
    private WeaponBrain weaponBrain;// 如果你有武器系统的话

    private void Awake()
    {
        // 1. 获取所有子组件 (此时子组件们在安静地等待)
        Locomotion = GetComponent<PlayerLocomotion>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Health = GetComponent<PlayerHealth>();
        Experience = GetComponent<PlayerExperience>();
        Visuals = GetComponent<PlayerVisuals>();
        TryGetComponent(out weaponBrain); // 武器系统可能不是每个角色都有，所以用 TryGetComponent
        // 2. 初始化核心血液系统
        // 这里完美落实了白皮书中提到的“根据基础 SO 实例化面板”
        Stats = new StatCalculator(baseStatsSO); 

        // 3. 核心大阅兵：主动向下注入依赖 (Push)
        // 顺序完全由你掌控！比如必须先初始化 Input，再初始化 Locomotion
        InputHandler.Initialize(); // 如果输入系统需要初始化的话
        Locomotion.Initialize(this);
        Health.Initialize(this);
        Experience.Initialize(this);
        Visuals.Initialize(this);
        if (weaponBrain != null)
            weaponBrain.Initialize(this);
    }

    public void OnEnable()
    {
        if(playerTransformAnchor != null) 
            playerTransformAnchor.Value = transform;
    }

    public void OnDisable()
    {
        if(playerTransformAnchor != null) 
            playerTransformAnchor.Value = null;
    }
}