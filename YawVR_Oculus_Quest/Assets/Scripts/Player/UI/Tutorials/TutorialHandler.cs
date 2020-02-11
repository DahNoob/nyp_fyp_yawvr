using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialHandler : MonoBehaviour
{
    public static TutorialHandler instance;

    [System.Serializable]
    public struct PlayerTutorialElements
    {
        public GameObject playerTutorialUI;
        public TextMeshProUGUI m_tutorialText;
        public TextMeshProUGUI m_tutorialHeader;
        public Transform leftTransform;
        public Transform rightTransform;
        public Transform middleTransform;
    }

    //Linked stuff first

    public enum TUTORIAL_TYPE
    {
        RELOAD,
        MOVE,
        RESET_POSE,
        WEAPONS,
        CHANGE_WEAPON,
        SHOOTING,
        LOOKAROUND,
        LIGHTMECH2,
        HEAVYMECH2,
        OBJECTIVES,
        DEFAULT
    }

    [SerializeField]
    private List<TutorialInfo> m_tutorialInfo;

    //Linked things
    [SerializeField]
    private PlayerTutorialElements m_elements;

    //Local variables
    Queue<TutorialMessage> m_tutorialQueue = new Queue<TutorialMessage>();
    Queue<TutorialMessage> m_processingQueue = new Queue<TutorialMessage>();

    private Dictionary<int, TutorialInfo> m_tutorialDictionary = new Dictionary<int, TutorialInfo>();

    static int MAX_TUTORIAL_QUEUE_COUNT = 12;
    private int m_tutorialCount = 0;
    private bool isAlreadyTyping = false;
    private string currentString;

    private TutorialInfo previousTutorialInfo;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if(m_elements.playerTutorialUI != null)
            m_elements.playerTutorialUI.SetActive(false);

        //Add everything to the dictionary
        for (int i = 0; i < m_tutorialInfo.Count; ++i)
        {
            m_tutorialDictionary.Add((int)m_tutorialInfo[i].tutorialType, m_tutorialInfo[i]);

            if(!m_tutorialInfo[i].singleInput)
            {
                //Instantiate the objects based on...
                if (m_tutorialInfo[i].leftControllerPrefab != null)
                {
                    m_tutorialInfo[i].leftController = Instantiate(m_tutorialInfo[i].leftControllerPrefab, m_elements.leftTransform.position, Quaternion.identity, m_elements.leftTransform);
                    m_tutorialInfo[i].leftController.transform.localEulerAngles = Vector3.zero;
                    m_tutorialInfo[i].leftController.transform.localPosition = Vector3.zero;
                    m_tutorialInfo[i].leftController.SetActive(false);
                }
                if (m_tutorialInfo[i].rightControllerPrefab != null)
                {
                    m_tutorialInfo[i].rightController = Instantiate(m_tutorialInfo[i].rightControllerPrefab, m_elements.rightTransform.position, Quaternion.identity, m_elements.rightTransform);
                    m_tutorialInfo[i].rightController.transform.localEulerAngles = Vector3.zero;
                    m_tutorialInfo[i].rightController.transform.localPosition = Vector3.zero;
                    m_tutorialInfo[i].rightController.SetActive(false);
                }
            }
            else
            {
                //Instantiate the objects based on...
                if (m_tutorialInfo[i].leftControllerPrefab != null)
                {
                    m_tutorialInfo[i].leftController = Instantiate(m_tutorialInfo[i].leftControllerPrefab, m_elements.middleTransform.position, Quaternion.identity, m_elements.middleTransform);
                    m_tutorialInfo[i].leftController.transform.localEulerAngles = Vector3.zero;
                    m_tutorialInfo[i].leftController.transform.localPosition = Vector3.zero;
                    m_tutorialInfo[i].leftController.SetActive(false);
                }
                if (m_tutorialInfo[i].rightControllerPrefab != null)
                {
                    m_tutorialInfo[i].rightController = Instantiate(m_tutorialInfo[i].rightControllerPrefab, m_elements.middleTransform.position, Quaternion.identity, m_elements.middleTransform);
                    m_tutorialInfo[i].rightController.transform.localEulerAngles = Vector3.zero;
                    m_tutorialInfo[i].rightController.transform.localPosition = Vector3.zero;
                    m_tutorialInfo[i].rightController.SetActive(false);
                }
            }

        }
    }

    public void AddTutorial(TUTORIAL_TYPE type)
    {
        int tag = (int)type;

        TutorialInfo currentTutorialInfo = m_tutorialDictionary[tag];

        if (currentTutorialInfo != null)
        {
            if (previousTutorialInfo != null)
            {
                if (previousTutorialInfo.leftController != null)
                    previousTutorialInfo.leftController.SetActive(false);
                if (previousTutorialInfo.rightController != null)
                    previousTutorialInfo.rightController.SetActive(false);
            }

            //Enable things
            m_elements.m_tutorialHeader.text = currentTutorialInfo.m_tutorialHeader;

            if (currentTutorialInfo.leftController != null)
                currentTutorialInfo.leftController.SetActive(true);
            if (currentTutorialInfo.rightController != null)
                currentTutorialInfo.rightController.SetActive(true);

            //Set previous to current
            previousTutorialInfo = currentTutorialInfo;

            for (int i = 0; i < currentTutorialInfo.m_tutorialMessages.Count; ++i)
            {
                AddStringToProcessingQueue(currentTutorialInfo.m_tutorialMessages[i]);
            }

            m_elements.m_tutorialText.text = "";
            //Enable the overall overlay
            m_elements.playerTutorialUI.SetActive(true);
        }
    }

    public void EndTutorial(TUTORIAL_TYPE tyoe)
    {
        m_elements.m_tutorialText.text = currentString;
        m_elements.playerTutorialUI.SetActive(false);
    }


    public void AddStringToProcessingQueue(TutorialMessage tutorialMessage)
    {
        if (isAlreadyTyping)
            m_processingQueue.Enqueue(tutorialMessage);
        else
            AddStringToTutorialQueue(tutorialMessage);
    }

    bool AddStringToTutorialQueue(TutorialMessage tutorialMessage)
    {
        if (m_tutorialCount < MAX_TUTORIAL_QUEUE_COUNT)
        {
            m_tutorialQueue.Enqueue(tutorialMessage);
            //Add one more to the thingy
            StartCoroutine(TypeNewTutorialLine(tutorialMessage));
            m_tutorialCount++;
        }
        else
        {
            m_tutorialQueue.Dequeue();
            //AssignLastText();
            StartCoroutine(TypeNewTutorialLine(tutorialMessage));
            m_tutorialQueue.Enqueue(tutorialMessage);
        }

        return true;
    }

    IEnumerator TypeNewTutorialLine(TutorialMessage tutorialMessage)
    {
        isAlreadyTyping = true;

        yield return new WaitForSeconds(tutorialMessage.delay);

        m_elements.m_tutorialText.text = "";
        currentString = tutorialMessage.message;
        //Set the text in player to be ""
        foreach (char letter in tutorialMessage.message)
        {
            //Text += message
            m_elements.m_tutorialText.text += letter;
            yield return new WaitForSeconds(tutorialMessage.messageSpeed);

            if(m_elements.m_tutorialText.text == tutorialMessage.message)
            {
                isAlreadyTyping = false;
                m_tutorialCount--;
                yield break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 49; i < 56; ++i)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                AddTutorial((TUTORIAL_TYPE)i - 49);
            }
        }

        //yeet
        if (m_processingQueue.Count > 0 && !isAlreadyTyping)
        {
            AddStringToTutorialQueue(m_processingQueue.Dequeue());
        }
    }
}

[System.Serializable]
public class TutorialInfo
{
    //For editor
    public string name;
    //Type
    public TutorialHandler.TUTORIAL_TYPE tutorialType;
    //The header
    public string m_tutorialHeader;
    //List of things, and type
    public List<TutorialMessage> m_tutorialMessages;
    //Prefab
    public GameObject leftControllerPrefab;
    //Right prefab
    public GameObject rightControllerPrefab;

    public bool singleInput = true;

    //Finalized GameObjects
    [HideInInspector]
    public GameObject leftController;
    [HideInInspector]
    public GameObject rightController;

}

[System.Serializable]
public class TutorialMessage
{
    [TextArea(3, 10)]
    public string message;
    public float delay;
    public float messageSpeed;

    public TutorialMessage(string _message, float _delay, float _messageSpeed)
    {
        message = _message;
        delay = _delay;
        messageSpeed = _messageSpeed;
    }
}
