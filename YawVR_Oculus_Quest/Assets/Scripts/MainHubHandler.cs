using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainHubHandler : MonoBehaviour
{
    [SerializeField]
    private WorldPickable m_chairPickable;
    [SerializeField]
    private OVRScreenFade m_playerScreenFade;
    [SerializeField]
    private Transform m_hologramsRoot;
    [SerializeField]
    private GameObject[] m_planets_prefabs;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_planetNameUi;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_coinsDisplayText;
    [SerializeField]
    private OVR.SoundFXRef m_spaceBgm;
    [SerializeField]
    private OVR.SoundFXRef m_welcomeAboardSound;
    [SerializeField]
    private OVR.SoundFXRef m_welcomeBackSound;
    [SerializeField]
    private OVR.SoundFXRef m_nowTravellingSound;
    [SerializeField]
    private OVR.SoundFXRef m_clickUiSound;
    [SerializeField]
    private OVR.SoundFXRef m_hoverUiSound;
    
    private bool isChangingScene = false;
    private BasePlanetHolograph[] planets;
    private int currentPlanetIndex = 0;
    private PlanetHolographPickable planetHoloPickable;
    private float welcomeTimer = 0;
    private bool hasBeenWelcomed = false;

    void Awake()
    {
        m_chairPickable.onSelected += _chairPickable_onSelected;
        System.Array.Resize(ref planets, m_planets_prefabs.Length);
        for (int i = 0; i < planets.Length; ++i)
        {
            planets[i] = Instantiate(m_planets_prefabs[i], m_hologramsRoot).GetComponent<BasePlanetHolograph>();
            if (i != currentPlanetIndex)
                planets[i].gameObject.SetActive(false);
        }
        m_planetNameUi.text = planets[currentPlanetIndex].m_planetName;
        planetHoloPickable = m_hologramsRoot.GetComponent<PlanetHolographPickable>();
    }

    void Start()
    {
        m_spaceBgm.PlaySound(Random.Range(1.0f, 2.0f));
        m_spaceBgm.AttachToParent(Camera.main.transform);
        if (Persistent.instance.isFirstTime)
        {
            m_welcomeAboardSound.PlaySound(2);
            m_welcomeAboardSound.AttachToParent(Camera.main.transform);
        }
        else
        {
            m_welcomeBackSound.PlaySound(2);
            m_welcomeBackSound.AttachToParent(Camera.main.transform);
        }
        m_coinsDisplayText.text = PlayerPrefs.GetInt("Currency", 0).ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            _chairPickable_onSelected();
        if (Input.GetKeyDown(KeyCode.Z))
            PreviousPlanet();
        if (Input.GetKeyDown(KeyCode.C))
            NextPlanet();
    }

    void OnDisable()
    {
        m_chairPickable.onSelected -= _chairPickable_onSelected;
        
    }

    void _chairPickable_onSelected()
    {
        if (isChangingScene) return;
        isChangingScene = true;
        Persistent.instance.isFirstTime = false;
        StartCoroutine(fadeToScene(planets[currentPlanetIndex].m_sceneName));
    }

    public void NextPlanet()
    {
        currentPlanetIndex++;
        if (currentPlanetIndex >= planets.Length)
            currentPlanetIndex = 0;
        UpdatePlanet();
    }

    public void PreviousPlanet()
    {
        currentPlanetIndex--;
        if (currentPlanetIndex < 0)
            currentPlanetIndex = planets.Length - 1;
        UpdatePlanet();
    }

    void UpdatePlanet()
    {
        for (int i = 0; i < planets.Length; ++i)
        {
            planets[i].gameObject.SetActive(false);
            if (i == currentPlanetIndex)
            {
                planets[i].gameObject.SetActive(true);
                m_planetNameUi.text = planets[i].m_planetName;
            }
        }
        planetHoloPickable.ResetFade();
    }

    public void PlayHoverUiSound()
    {
        m_hoverUiSound.PlaySoundAt(Camera.main.transform.position);
    }
    public void PlayClickUiSound()
    {
        m_clickUiSound.PlaySoundAt(Camera.main.transform.position);
    }

    IEnumerator fadeToScene(string _sceneName)
    {
        m_nowTravellingSound.PlaySound();
        m_nowTravellingSound.AttachToParent(Camera.main.transform);
        m_playerScreenFade.fadeTime = 1.5f;
        m_playerScreenFade.FadeOut();
        m_spaceBgm.DetachFromParent();
        m_spaceBgm.StopSound();
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(_sceneName);
    }
}
