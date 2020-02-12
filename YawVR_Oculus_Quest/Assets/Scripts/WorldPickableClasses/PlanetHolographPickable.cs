using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHolographPickable : WorldPickable
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_textUi;

    private Coroutine textFadeCoroutine;
    private float timer = 999;

    void Start()
    {
        m_textUi.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        if(isHighlighted)
        {
            ResetFade();
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 6 && textFadeCoroutine == null)
                textFadeCoroutine = StartCoroutine(fadeText());
        }
    }

    override public void SetHighlighted(bool _var)
    {
        isHighlighted = _var;
    }

    public void ResetFade()
    {
        timer = 0;
        if (textFadeCoroutine != null)
        {
            StopCoroutine(textFadeCoroutine);
            textFadeCoroutine = null;
        }
    }

    IEnumerator fadeText()
    {
        timer = -float.MaxValue;
        m_textUi.color = Color.white;
        for (int i = 0; i < 10; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            m_textUi.color = new Color(1, 1, 1, 1 - (i / 10.0f));
        }
        m_textUi.color = new Color(1, 1, 1, 0);
        textFadeCoroutine = null;
    }
}
