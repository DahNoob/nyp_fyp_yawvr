using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OVR;

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
        public GameObject continueButton;
    }

    //Linked stuff first

    public enum TUTORIAL_TYPE
    {
        WELCOME_TUTORIAL,
        LOOKAROUND1,
        LOOKAROUND2,
        MOVING,
        READYINGWEAPONS,
        RELOADING,
        RESETPOSE,
        SWAP_WEAPONS,
        SHOOTING_WEAPONS,
        DEFAULT
    }

    [SerializeField]
    private List<TutorialInfo> m_tutorialInfo;

    //Linked things
    [SerializeField]
    private PlayerTutorialElements m_elements;

    //Local variables
    Queue<TutorialInfo> m_tutorialQueue = new Queue<TutorialInfo>();

    private Dictionary<int, TutorialInfo> m_tutorialDictionary = new Dictionary<int, TutorialInfo>();

    private bool m_finishedCurrent = false;
    public bool m_isWaiting = false;
    private string currentString;

    private TutorialInfo previousTutorialInfo;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (m_elements.playerTutorialUI != null)
            m_elements.playerTutorialUI.SetActive(false);

        //Add everything to the dictionary
        for (int i = 0; i < m_tutorialInfo.Count; ++i)
        {
            m_tutorialDictionary.Add((int)m_tutorialInfo[i].tutorialType, m_tutorialInfo[i]);

            if (!m_tutorialInfo[i].singleInput)
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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; ++i)
        {
                AddTutorial((TUTORIAL_TYPE)i);
        }
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            Continue();
        }
    }

    public void AddTutorial(TUTORIAL_TYPE type)
    {
        int tag = (int)type;
        TutorialInfo currentTutorialInfo = m_tutorialDictionary[tag];
        if (previousTutorialInfo == null)
        {
            //If there is no current one
            //Enable sprites
            if (currentTutorialInfo != null)
            {
                if (currentTutorialInfo.leftController != null)
                    currentTutorialInfo.leftController.SetActive(true);
                if (currentTutorialInfo.rightController != null)
                    currentTutorialInfo.rightController.SetActive(true);

                //Set header
                m_elements.m_tutorialHeader.text = currentTutorialInfo.m_tutorialHeader;

                StartCoroutine(TypeNewTutorialLine(currentTutorialInfo));

                //Enable the thing
                m_elements.playerTutorialUI.SetActive(true);
                m_elements.continueButton.SetActive(true);

                //Set width
                m_elements.m_tutorialText.GetComponent<RectTransform>().sizeDelta = currentTutorialInfo.textBoxWidth;
                //Play Sound
                if (currentTutorialInfo.soundType != PlayerUISoundManager.TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
                {
                    SoundData soundData = currentTutorialInfo.m_soundData;
                    PlayerUISoundManager.instance.PlaySound(currentTutorialInfo.soundType, soundData.delaySecs, soundData.volume);
                }
                previousTutorialInfo = currentTutorialInfo;
            }
        }
        else
        {
            //Add to the queue for processing later
            m_tutorialQueue.Enqueue(currentTutorialInfo);
        }
    }

    //Internal function
    private void FetchNewInfo()
    {
        if (m_tutorialQueue != null && m_tutorialQueue.Count > 0)
        {
            TutorialInfo currentTutorialInfo = m_tutorialQueue.Dequeue();
            if (currentTutorialInfo != null)
            {
                if (previousTutorialInfo != null)
                {
                    if (previousTutorialInfo.leftController != null)
                        previousTutorialInfo.leftController.SetActive(false);
                    if (previousTutorialInfo.rightController != null)
                        previousTutorialInfo.rightController.SetActive(false);

                    //Enable things
                    m_elements.m_tutorialHeader.text = currentTutorialInfo.m_tutorialHeader;

                    if (currentTutorialInfo.leftController != null)
                        currentTutorialInfo.leftController.SetActive(true);
                    if (currentTutorialInfo.rightController != null)
                        currentTutorialInfo.rightController.SetActive(true);

                    m_elements.m_tutorialText.text = "";
                    //Enable the overall overlay
                    m_elements.playerTutorialUI.SetActive(true);
                    m_elements.continueButton.SetActive(true);

                    //Start the typing thingo
                    StartCoroutine(TypeNewTutorialLine(currentTutorialInfo));

                    //Set width
                    m_elements.m_tutorialText.GetComponent<RectTransform>().sizeDelta = currentTutorialInfo.textBoxWidth;

                    //Stop previous sound and play new sound.
                    if (previousTutorialInfo != null && previousTutorialInfo.soundType != PlayerUISoundManager.TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
                    {
                        PlayerUISoundManager.instance.StopSound(previousTutorialInfo.soundType);
                    }

                    //Play Sound
                    if (currentTutorialInfo.soundType != PlayerUISoundManager.TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
                    {
                        SoundData soundData = currentTutorialInfo.m_soundData;
                        PlayerUISoundManager.instance.PlaySound(currentTutorialInfo.soundType, soundData.delaySecs, soundData.volume);
                    }

                    //Set the previous to the current
                    previousTutorialInfo = currentTutorialInfo;
                }
            }
        }
        else
        {
            //Set active the playerUI;
            m_elements.m_tutorialText.text = "";
            m_elements.playerTutorialUI.SetActive(false);
            m_elements.continueButton.SetActive(false);
            //Set previous to be null, kinda cheat way... but yea
            if (previousTutorialInfo.leftController != null)
                previousTutorialInfo.leftController.SetActive(false);
            if (previousTutorialInfo.rightController != null)
                previousTutorialInfo.rightController.SetActive(false);
            previousTutorialInfo = null;
        }
    }

    public void Continue()
    {
        if (m_isWaiting)
            m_isWaiting = false;
        else
        {
            if (previousTutorialInfo != null && previousTutorialInfo.soundType != PlayerUISoundManager.TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
            {
                PlayerUISoundManager.instance.StopSound(previousTutorialInfo.soundType);
            }
            m_elements.m_tutorialText.text = currentString;
            m_isWaiting = true;
        }
    }

    IEnumerator TypeNewTutorialLine(TutorialInfo tutorialInfo)
    {
        m_finishedCurrent = false;
        for (int i = 0; i < tutorialInfo.m_tutorialMessages.Count; ++i)
        {
            while (m_isWaiting)
            {
                yield return new WaitForEndOfFrame();
            }

            //Then move on to next message if possible
            yield return new WaitForSeconds(tutorialInfo.m_tutorialMessages[i].delay);

            m_elements.m_tutorialText.text = "";
            currentString = tutorialInfo.m_tutorialMessages[i].message;
            //Set the text in player to be ""
            foreach (char letter in tutorialInfo.m_tutorialMessages[i].message)
            {
                //To handle the skip
                if (m_elements.m_tutorialText.text == tutorialInfo.m_tutorialMessages[i].message)
                {
                    m_isWaiting = true;
                    break;
                }
                else
                    m_elements.m_tutorialText.text += letter;

                yield return new WaitForSeconds(tutorialInfo.m_tutorialMessages[i].messageSpeed);
            }

            if (m_elements.m_tutorialText.text == tutorialInfo.m_tutorialMessages[i].message)
            {
                m_isWaiting = true;
            }
        }

        //Waita gain after end of loop for final instructions.
        while (m_isWaiting)
        {
            yield return new WaitForEndOfFrame();
        }

        m_finishedCurrent = true;
        FetchNewInfo();
        yield break;
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
    //To show one sprite or not...
    public bool singleInput = true;
    //Prefab
    public GameObject leftControllerPrefab;
    //Right prefab
    public GameObject rightControllerPrefab;

    public Vector2 textBoxWidth = new Vector2(90f, 100f);

    //Audio Clip
    public PlayerUISoundManager.TUTORIAL_SOUNDTYPE soundType = PlayerUISoundManager.TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE;
    //SoundData
    public SoundData m_soundData;


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
