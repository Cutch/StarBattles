using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles
{
    public class Asteroid : MonoBehaviour {
        // Use this for initialization
        void Start()
        {
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            rb.AddTorque(rb.mass * 1000 * UnityEngine.Random.Range(-2.0f, 2.0f));
        }
    }
}