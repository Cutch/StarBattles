using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles
{
    public class DamageObject : MonoBehaviour
    {
        Rigidbody2D body;
        public Vector3 positionSpeed;
        public double damage;
        Vector3 orgPos;
        int enemyLayer;
        int staticLayer;
        // Use this for initialization
        void Start()
        {
            orgPos = transform.position;
            lastTime = Time.time;
            enemyLayer = LayerMask.NameToLayer("EnemyShips");
            staticLayer = LayerMask.NameToLayer("StaticLayer");
            body = GetComponent<Rigidbody2D>();
            body.velocity = this.positionSpeed;

        }

        float lastTime = -1;
        // Update is called once per frame
        void Update()
        {
            //float frameSeconds = (Time.time - lastTime);
            //lastTime = Time.time;
            //this.gameObject.transform.position += this.positionSpeed * frameSeconds;
            if ((transform.position - orgPos).magnitude > 30)
            {
                Destroy(gameObject);
            }
        }
        void OnCollisionEnter2D(Collision2D coll)
        {
            Debug.Log("Collidered, "+ damage);
            Debug.Log(coll.gameObject.tag);
            //if (coll.gameObject.tag != "MyShip" || coll.gameObject.tag != "DamageObject")
            if (coll.collider.gameObject.layer == enemyLayer)
            {
                Destroy(gameObject);
                coll.collider.gameObject.SendMessage("ApplyDamage", damage);
            }
            else if (coll.collider.gameObject.layer == staticLayer)
            {
                Destroy(gameObject);
            }
        }
    }
}