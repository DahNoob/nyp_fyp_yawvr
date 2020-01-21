using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [System.Serializable]
    public struct SystemFluffMessage
    {
        public string message;
        public float delay;
        public float messageSpeed;

        public SystemFluffMessage(string _message, float _delay, float _messageSpeed)
        {
            message = _message;
            delay = _delay;
            messageSpeed = _messageSpeed;
        }
    }

    //Honestly liek idk what is this tbh liek if i have time i will get back to this but...
    public static PlayerUIManager instance;

    [Header("Player Minimap")]
    [SerializeField]
    [Tooltip("Player's Minimap Component")]
    private PlayerUIMinimap m_playerMinimap;

    [Header("Minimap Icon Configuration")]
    [SerializeField]
    [Tooltip("Player Minimap Icons Configuration")]
    private PlayerUIMinimapIcons m_playerMinimapIcons;

    [Header("Player HUD Configuration")]
    //Will move to class later if there is time
    [SerializeField]
    [Tooltip("HUD Text")]
    UnityEngine.UI.Text m_systemFluffs;

    [SerializeField]
    [Tooltip("Starting Fluffs")]
    private List<SystemFluffMessage> m_startingSystemFluffs;

    //Local variables
    Queue<SystemFluffMessage> m_systemQueue = new Queue<SystemFluffMessage>();
    Queue<SystemFluffMessage> m_processingQueue = new Queue<SystemFluffMessage>();
    static int MAX_SYSTEM_QUEUE_COUNT = 3;
    private int m_systemFluffCount = 0;
    private string m_previousSystemText;
    private bool isAlreadyTyping = false;

    //Get stuff
    public PlayerUIMinimap playerMinimap
    {
        get { return m_playerMinimap; }
        private set { }
    }
    //Get stuff
    public PlayerUIMinimapIcons playerMinimapIcons
    {
        get { return m_playerMinimapIcons; }
        private set { }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        m_playerMinimapIcons.Awake();
        //Check null functio nfor the icons
        StartCoroutine(m_playerMinimapIcons.CheckNull());
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerMinimap.Start();

        foreach (SystemFluffMessage startingFluffs in m_startingSystemFluffs)
        {
            AddStringToProcessingQueue(startingFluffs);
        }

    }
    // Update is called once per frame
    void Update()
    {
        m_playerMinimap.Update();
        m_playerMinimapIcons.Update();

        if (m_processingQueue.Count > 0 && !isAlreadyTyping)
        {
            AddStringToSystemQueue(m_processingQueue.Dequeue());
        }
        else if (m_processingQueue.Count == 0 && !isAlreadyTyping)
        {

        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    AddStringToProcessingQueue(FormatFluff("Interesting, to say the least."));
        //}
    }

    public void AddStringToProcessingQueue(SystemFluffMessage fluffs)
    {
        if (isAlreadyTyping)
            m_processingQueue.Enqueue(fluffs);
        else
            AddStringToSystemQueue(fluffs);
    }

    public bool AddStringToSystemQueue(SystemFluffMessage fluffs)
    {
        fluffs.message = FormatFluff(fluffs.message);
        if (m_systemFluffCount < MAX_SYSTEM_QUEUE_COUNT)
        {
            m_systemQueue.Enqueue(fluffs);
            //Add one more to the thingy
            StartCoroutine(TypeNewSystemLine(fluffs));
            m_systemFluffCount++;
        }
        else
        {
            m_systemQueue.Dequeue();
            AssignLastText();
            StartCoroutine(TypeNewSystemLine(fluffs));
            m_systemQueue.Enqueue(fluffs);
        }

        return true;
    }

    public string AssignLastText()
    {
        if (m_systemQueue.Count == 0)
            return "";

        m_previousSystemText = "";
        Queue<SystemFluffMessage> newQueue = new Queue<SystemFluffMessage>(m_systemQueue);
        while (newQueue.Count > 0)
        {
            m_previousSystemText += newQueue.Dequeue().message;
        }

        m_systemFluffs.text = m_previousSystemText + "\n";

        return m_previousSystemText;
    }

    IEnumerator TypeNewSystemLine(SystemFluffMessage fluff)
    {
        isAlreadyTyping = true;

        yield return new WaitForSeconds(fluff.delay);

        foreach (char letter in fluff.message)
        {
            string previousText = "";
            char[] systemFluffArray = m_systemFluffs.text.ToCharArray();

            for (int i = 0; i < systemFluffArray.Length - 1; ++i)
            {
                previousText += systemFluffArray[i];
            }
            m_systemFluffs.text = previousText;

            m_systemFluffs.text += letter + "|";
            //m_systemFluffs.text += letter;
            yield return new WaitForSeconds(fluff.messageSpeed);
        }
        isAlreadyTyping = false;
    }


    string FormatFluff(string message)
    {
        return "-" + message + "\n";
    }
}
