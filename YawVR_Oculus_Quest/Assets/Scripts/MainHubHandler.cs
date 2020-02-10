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

    private bool isChangingScene = false;

    void Awake()
    {
        m_chairPickable.onSelected += _chairPickable_onSelected;
    }

    void OnDisable()
    {
        m_chairPickable.onSelected -= _chairPickable_onSelected;
    }

    void _chairPickable_onSelected()
    {
        if (isChangingScene) return;
        isChangingScene = true;
        StartCoroutine(fadeToScene("NewDesertMap"));
    }

    IEnumerator fadeToScene(string _sceneName)
    {
        m_playerScreenFade.fadeTime = 1.5f;
        m_playerScreenFade.FadeIn();
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(_sceneName);
    }
}
