using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    private float mass = 3.0f;
    private Vector3 impact = Vector3.zero;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (impact.magnitude > 0.2)
        {
            character.Move(impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if(direction.y > 0)
        {
            // reflect down force on the ground
            //direction.x = -direction.x;
            //direction.z = -direction.z;
            direction.y = -direction.y;
            impact += direction.normalized * force / mass;
        }
    }
}
