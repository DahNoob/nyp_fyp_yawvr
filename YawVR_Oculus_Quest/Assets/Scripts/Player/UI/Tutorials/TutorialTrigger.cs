using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    private TutorialHandler.TUTORIAL_TYPE m_tutorialType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            TutorialHandler.instance.AddTutorial(m_tutorialType);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            TutorialHandler.instance.EndTutorial(m_tutorialType);
    }

}
