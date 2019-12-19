using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Weighted Random
** Desc: To provide a weighted randomize rarity for enemies when they spawn
** Author: Wei Hao
** Date: 6/12/2019, 9:15 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 10:30 PM     Wei Hao   Created and implemented
*******************************/
public class WeightedRandom : MonoBehaviour
{
    int[] weights;
    int weightTotal;

    struct Rarity
    {
        public const int noRarity = 0;
        // Uncommon (1 passives)
        public const int delta = 1;
        // Rare (2 passives)
        public const int beta = 2;
        // Epic (3 passives)
        public const int omega = 3;
        // Legendary (4 passives)
        public const int alpha = 4;
        
    }

    void Awake()
    {
        weights = new int[5]; // Total number of rarity

        //weighting of each rarity, high number means more occurrance
        weights[Rarity.noRarity] = 50;
        weights[Rarity.delta] = 25;
        weights[Rarity.beta] = 13;
        weights[Rarity.omega] = 7;
        weights[Rarity.alpha] = 5;

        weightTotal = 0;
        foreach (int w in weights)
        {
            weightTotal += w;
        }
    }

    public int random()
    {
        int result = 0;
        int total = 0;
        int randomValue = Random.Range(0, weightTotal + 1);
        for (result = 0; result < weights.Length; result++)
        {
            total += weights[result];
            if (total >= randomValue) break;
        }
        return result;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("First value is: " + random());
        //Debug.Log("Second value is: " + random());
        //Debug.Log("Third value is: " + random());
        //Debug.Log("Fourth value is: " + random());
        //Debug.Log("Fifth value is: " + random());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
