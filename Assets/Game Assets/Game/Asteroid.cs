using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles{
    public class Asteroid : MonoBehaviour {

        Rigidbody2D rb;
        Vector2 velocity;
        float radialVelocity;
        // Use this for initialization
        void Start()
        {
            rb = this.GetComponent<Rigidbody2D>();
            rb.angularVelocity = UnityEngine.Random.Range(-2.0f, 2.0f);
            rb.velocity = new(UnityEngine.Random.Range(-2.0f, 2.0f), UnityEngine.Random.Range(-2.0f, 2.0f));
        }

        void Update()
        {
            float frameSeconds = Time.deltaTime;
            rb.angularVelocity = this.radialVelocity;
            rb.velocity = this.velocity;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.)
        }
    }
}