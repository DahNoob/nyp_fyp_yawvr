using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIObjectiveHandler
{
    public static UIObjectiveHandler instance { private set; get; }

    [Header("Player UI Objectives Configuration")]
    [SerializeField]
    private bool m_componentEnabled;

    [Header("UI Objective Animations")]
    [SerializeField]
    private List<RectTransform> m_objectiveTransformList;
    [SerializeField]
    private RectTransform m_objectiveHex;
    [SerializeField]
    private Animator targetAcquiredAnimator;
    [SerializeField]
    private Animator objectiveHexAnimator;
    [SerializeField]
    [Range(0, 1f)]
    private float lerpTimeOffset;

    [Header("UI Objectives Panel Configuration")]
    [SerializeField]
    private ObjectivesGUIInfo[] m_objectivesGUI;
    [SerializeField]
    private Sprite lockedObjectiveImage;
    [SerializeField]
    private Sprite failedObjectiveImage;
    [SerializeField]
    private Sprite successObjectiveImage;
    [SerializeField]
    private Sprite baseObjectiveImage;
    [SerializeField]
    private RectTransform m_objectiveArrow;

    //Current objective info
    private ObjectiveInfo m_activeObjective;


    //Local variables
    private float lerpTime;
    //Desired rect
    private RectTransform desiredRectTransform;
    //Prev locations
    private Vector3 prevPosition;
    private Quaternion prevRotation;

    void OnEnable()
    {
        //if (Game.instance)
        //{
        //    Game.instance.onObjectiveStarted += Game_onObjectiveStarted;
        //    Game.instance.onObjectiveFinished += Game_onObjectiveFinished;
        //    //print("GUIManager events +attached+ successfully!");
        //}
    }

    void OnDisable()
    {
        Game.instance.onObjectiveStarted -= Game_onObjectiveStarted;
        Game.instance.onObjectiveFinished -= Game_onObjectiveFinished;
        Debug.Log("Game events -detached- successfully!");
    }

    // Start is called before the first frame update
    public void Awake()
    {
        if (!m_componentEnabled)
            return;

        if (instance == null)
            instance = this;

        prevPosition = m_objectiveHex.anchoredPosition3D;
        prevRotation = m_objectiveHex.localRotation;
        m_objectiveArrow.gameObject.SetActive(false);
    }

    public void Start()
    {
        //Game.instance.onObjectiveStarted += Game_onObjectiveStarted;
        //Game.instance.onObjectiveFinished += Game_onObjectiveFinished;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!m_componentEnabled)
            return;

        //UpdateObjectiveArrow();

        //We have a target
        if (desiredRectTransform != null)
        {
            if (AnimatorIsCurrentState(objectiveHexAnimator, "ObjectiveHexFadeIn")
                && !AnimatorIsPlaying(objectiveHexAnimator, "ObjectiveHexFadeIn"))
            {
                //lerp towards the thing
                lerpTime += Time.deltaTime;

                m_objectiveHex.anchoredPosition3D = Vector3.Lerp(m_objectiveHex.anchoredPosition3D, desiredRectTransform.anchoredPosition3D, lerpTime);

                m_objectiveHex.rotation = Quaternion.Lerp(m_objectiveHex.rotation, desiredRectTransform.rotation, lerpTime);

                //Finished animation
                if (lerpTime >= lerpTimeOffset)
                {
                    objectiveHexAnimator.Play("ObjectiveHexExpand");
                }

            }
        }
    }

    public void ObjectiveTriggered(int objectiveIndex)
    {
        m_objectiveHex.anchoredPosition3D = prevPosition;
        m_objectiveHex.localRotation = prevRotation;
        //Reset lerp time
        lerpTime = 0;
        //Play the animator
        objectiveHexAnimator.Play("ObjectiveHexFadeIn");
        targetAcquiredAnimator.Play("TargetAcquiredFadeIn");

        desiredRectTransform = m_objectiveTransformList[objectiveIndex];

        PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.OBJECTIVE_TRIGGER);

    }

    public void SetActiveObjective(ObjectiveInfo _objectiveInfo = null)
    {
        //if (_objectiveInfo != null)
        //    m_objectiveArrow.gameObject.SetActive(true);

        if (_objectiveInfo != null && !_objectiveInfo.m_completed)
        {
            if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//srsly fuckin garbage
            {
                _objectiveInfo.panelInfo.panelText.text = "Bounty Hunt";
            }
            else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
            {
                _objectiveInfo.panelInfo.panelText.text = "Defend structure";
            }
        }

        m_activeObjective = _objectiveInfo;

        if (_objectiveInfo != null)
        {
            //Set the appropriate sprites
            //Remove lock icon from base
            _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
            _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(false);
            _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(true);
            _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(true);
        }
    }

    public void AddObjectiveToPanel(ref ObjectiveInfo _objectiveInfo, int index)
    {
        //Text panel = Instantiate(m_objectiveTextPanelPrefab, m_objectiveListPanel.transform).GetComponent<Text>();

        _objectiveInfo.panelInfo = m_objectivesGUI[index];
        if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//hhhhhhhhhhhhh this is such garbage code fajfjofifaoopzpovzxda
        {
            m_objectivesGUI[index].panelText.text = "Bounty Hunt";
        }
        else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            m_objectivesGUI[index].panelText.text = "Defend structure";
        }
    }

    public void FailedObjectiveGUI(ref ObjectiveInfo _objectiveInfo)
    {
        //Set the back black and the failed thing
        _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
        _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(true);
        _objectiveInfo.panelInfo.objectiveResult.sprite = failedObjectiveImage;

        //Disable the panel stuff
        _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(false);
        _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(false);

        //Play sound that failed
        PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.OBJECTIVE_FAILED);
        //m_objectivesGUI[index].panelFirstFill.color = Color.red;
        //m_objectivesGUI[index].panelSecondFill.color = Color.red;
        ////Set to 100%
        //m_objectivesGUI[index].panelProgress.color = Color.red;

    }

    public void SucceededObjectiveGUI(ref ObjectiveInfo _objectiveInfo)
    {
        //Set the back black and the failed thing
        _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
        _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(true);
        _objectiveInfo.panelInfo.objectiveResult.sprite = successObjectiveImage;

        _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(false);
        _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(false);

        //Play sound that suceed
        PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.OBJECTIVE_SUCCESS);
    }

    public void UpdateObjectiveProgress(ref ObjectiveInfo _objectiveInfo)
    {
        switch (_objectiveInfo.type)
        {
            case VariedObjectives.TYPE.BOUNTYHUNT:
                {
                    //m_objectiveTexts[index].backBlack.gameObject.SetActive(false);

                    //m_objectiveTexts[index].panelSecondFill.fillAmount = 1;
                    ////Set to 100%
                    //m_objectiveTexts[index].panelText.text = "100%";
                    break;
                }
            case VariedObjectives.TYPE.DEFEND_STRUCTURE:
                {
                    float fillAmount = 1 - (_objectiveInfo.m_timeLeft / _objectiveInfo.m_initialTime);
                    int fillAmountInt = (int)(fillAmount * 100);
                    _objectiveInfo.panelInfo.secondFill.fillAmount = fillAmount;
                    //Set to 100%
                    _objectiveInfo.panelInfo.panelProgress.text = fillAmountInt.ToString() + "%";
                    break;
                }
            case VariedObjectives.TYPE.TOTAL:
                break;
            default:
                break;
        }
    }

    public void UpdateObjectiveArrow()
    {
        if (m_objectiveArrow.gameObject.activeInHierarchy)
        {
            if (m_activeObjective == null)
            {
                if (m_objectiveArrow.gameObject.activeInHierarchy)
                    m_objectiveArrow.gameObject.SetActive(false);
            }
            else
            {
                if (m_activeObjective.m_highlight != null)
                {
                    Vector3 displacement = Vector3.Scale(m_activeObjective.m_highlight.position - PlayerHandler.instance.transform.position, new Vector3(1, 0, 1));
                    Vector3 bap = displacement.normalized;
                    float dist = displacement.magnitude;
                    float lol = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;
                    m_objectiveArrow.localPosition = Vector3.zero;
                    m_objectiveArrow.localEulerAngles = new Vector3(0, 0, lol + PlayerHandler.instance.transform.eulerAngles.y + 180);
                    //m_objectiveArrow.Rotate(90, 0, 0);
                    m_objectiveArrow.Translate(0, Mathf.Min(0.14f, displacement.sqrMagnitude * 0.00005f), 0);

                    string additionalInfo = m_activeObjective.m_inProgress ? "Time left : " + ((int)m_activeObjective.m_timeLeft) : "Proximity : " + ((int)dist);
                    if (m_activeObjective.type == VariedObjectives.TYPE.BOUNTYHUNT)//hhhhhhhhhhhhh this is such garbage code fajfjofifaoopzpovzxda
                    {
                        m_activeObjective.panelInfo.panelText.text = string.Format("Bounty hunt\n{0}", additionalInfo);
                    }
                    else if (m_activeObjective.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
                    {
                        m_activeObjective.panelInfo.panelText.text = string.Format("Defend structure\n{0}", additionalInfo);
                    }
                }
            }
        }
    }

    //Animator stuff
    bool AnimatorIsPlaying(Animator refAnimator, string stateName)
    {
        return refAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    bool AnimatorIsCurrentState(Animator refAnimator, string stateName)
    {
        return refAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    //Events
    public void Game_onObjectiveStarted(ObjectiveInfo _objectiveInfo)
    {

    }

    public void Game_onObjectiveFinished(ObjectiveInfo _objectiveInfo, bool _succeeded)
    {
        if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//srsly fuckin garbage
        {
            _objectiveInfo.panelInfo.panelText.text = "Bounty Hunt";
        }
        else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            _objectiveInfo.panelInfo.panelText.text = "Defend structure";
        }

        if (_succeeded)
        {
            _objectiveInfo.panelInfo.panelText.color = Color.green;
            _objectiveInfo.panelInfo.panelText.text += "\nSuccess!";

            //Update UI
            SucceededObjectiveGUI(ref _objectiveInfo);
        }
        else
        {
            _objectiveInfo.panelInfo.panelText.color = Color.red;
            _objectiveInfo.panelInfo.panelText.text += "\nFailed!";

            //Update UI
            FailedObjectiveGUI(ref _objectiveInfo);

        }
        if (Game.instance.IsObjectivesCleared())
            PlayerHandler.instance.TriggerAllObjectivesCleared();
        Game.instance.SetRandomObjective();
    }



}
