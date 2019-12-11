using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToPlayerPos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = PlayerHandler.instance.transform.localPosition;
        transform.localRotation = PlayerHandler.instance.transform.localRotation;
    }
}
