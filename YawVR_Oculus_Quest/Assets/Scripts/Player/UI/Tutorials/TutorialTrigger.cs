using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    private TutorialHandler.TUTORIAL_TYPE[] m_tutorialTypes;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < m_tutorialTypes.Length; ++i)
            {
                TutorialHandler.instance.AddTutorial(m_tutorialTypes[i]);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

}
