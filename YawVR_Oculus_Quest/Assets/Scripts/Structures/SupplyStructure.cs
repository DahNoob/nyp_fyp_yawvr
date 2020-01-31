﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyStructure : BaseStructure
{
    public override void Die()
    {
        PlayerHandler.instance.AddCurrency(m_structureInfo.currencyValue);
        base.Die();
    }
}