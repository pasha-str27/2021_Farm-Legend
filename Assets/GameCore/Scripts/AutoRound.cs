using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRound : MonoBehaviour
{
        public bool isRound;
    public float speed;

    public Vector3 velocity = new Vector3(0, 0, -1);

    void Update()
    {
        if (isRound)
        {
            transform.Rotate(velocity * speed * Time.deltaTime);
        }
    }
}
