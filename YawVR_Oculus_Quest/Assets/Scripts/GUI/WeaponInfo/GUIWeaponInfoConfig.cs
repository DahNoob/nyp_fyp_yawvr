using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GUIWeaponInfoConfig
{
    [SerializeField]
    [Tooltip("Weapon Sprite")]
    private Image m_weaponSprite;

    [SerializeField]
    [Tooltip("Weapon Name")]
    private Text m_weaponNameText;

    [SerializeField]
    [Tooltip("Weapon Ammo")]
    private Text m_weaponAmmoText;

    public Image weaponSprite
    {
        get { return m_weaponSprite; } private set { }
    }

    public Text weaponNameText
    {
        get { return m_weaponNameText; }
        private set { }
    }
    public Text weaponAmmoText
    {
        get { return m_weaponAmmoText; }
        private set { }
    }

    //Local variables

}
