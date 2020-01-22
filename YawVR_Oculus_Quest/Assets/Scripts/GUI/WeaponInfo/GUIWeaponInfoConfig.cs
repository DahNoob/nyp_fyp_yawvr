using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GUIWeaponInfoConfig
{
    [SerializeField]
    [Tooltip("Weapon Sprite")]
    private Image m_weaponSprite;

    [SerializeField]
    [Tooltip("Weapon Name")]
    private TextMeshProUGUI m_weaponNameText;

    [SerializeField]
    [Tooltip("Weapon Ammo")]
    private TextMeshProUGUI m_weaponAmmoText;

    [SerializeField]
    [Tooltip("Weapon Slider")]
    private Slider m_weaponAmmoSlider;

    public Image weaponSprite
    {
        get { return m_weaponSprite; } private set { }
    }

    public TextMeshProUGUI weaponNameText
    {
        get { return m_weaponNameText; }
        private set { }
    }
    public TextMeshProUGUI weaponAmmoText
    {
        get { return m_weaponAmmoText; }
        private set { }
    }

    public Slider weaponAmmoSlider
    {
        get { return m_weaponAmmoSlider; }
        private set { }
    }
    //Local variables

}
