using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public enum FOLLOW_MODES
    {

    };
    [Header("Minimap Configurations")]
    [SerializeField]
    private GameObject objectToFollow;
    [SerializeField]
    private float heightOffset = 5f;

    [Header("Follow Configurations")]
    [SerializeField]
    FOLLOW_MODES followModes;


    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = objectToFollow.transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.x + heightOffset, targetPosition.z);

        Quaternion targetRotation = objectToFollow.transform.rotation;
        transform.rotation = Quaternion.Euler(90f, targetRotation.eulerAngles.y, 0);

    }
}
