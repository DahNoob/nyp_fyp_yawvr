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

    private bool isChangingScene = false;
    private BasePlanetHolograph[] planets;
    private int currentPlanetIndex = 0;

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
    }

    void OnDisable()
    {
        m_chairPickable.onSelected -= _chairPickable_onSelected;
    }

    void _chairPickable_onSelected()
    {
        if (isChangingScene) return;
        isChangingScene = true;
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
                planets[i].gameObject.SetActive(true);
        }
    }

    IEnumerator fadeToScene(string _sceneName)
    {
        m_playerScreenFade.fadeTime = 1.5f;
        m_playerScreenFade.FadeOut();
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(_sceneName);
    }
}
