using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

abstract public class WorldPickable : MonoBehaviour
{
    public delegate void Selected();
    public event Selected onSelected;
    

    protected bool isHighlighted = false;

    abstract public void SetHighlighted(bool _var);

    public void TriggerSelect()
    {
        onSelected?.Invoke();
    }
}
