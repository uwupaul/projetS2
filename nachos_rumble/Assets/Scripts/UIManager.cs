using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public SettingsMenu settingsMenu;
    public bool EscapeMod;
    
    public static UIManager Instance;
    
    private void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre UIM exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        Instance = this;
        EscapeMod = false;

        SettingsMenu.DisableMouse();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            settingsMenu.Toggle();
    }

    public void HitEffect()
    {
        // dès qu'on prend des dégâts -> l'écran devient rouge
        // reprendre des dégâts allonge le temps pendant lequel l'écran sera rouge
        // mais n'influe pas sur 'l'animation'

        // rajouter l'effet de coloration rouge de l'écran
    }
}