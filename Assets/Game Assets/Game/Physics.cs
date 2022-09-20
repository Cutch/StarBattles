using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles{
    public class Physics : MonoBehaviour {
        GameShip gs;
        int enemyLayer;
        int staticLayer;
        // Use this for initialization
        void Start()
        {
            gs = GetComponentInParent<GameShip>();
            enemyLayer = LayerMask.NameToLayer("EnemyShips");
            staticLayer = LayerMask.NameToLayer("StaticObject");
        }
        // Update is called once per frame
        void Update() {

        }
        //private Vector2 Bounce(Vector2 velocity, Vector2 collisionNormal)
        //{
        //    var direction = Vector2.Reflect(velocity.normalized, collisionNormal);
        //    Debug.Log("direction " + direction);

        //    return direction;
        //}
        //void CalcNewSpeed(float weight1, Vector2 speed1, float weight2, Vector2 speed2, out Vector2 newSpeed1, out Vector2 newSpeed2)
        //{
        //    Debug.Log("speed1 " + speed1);
        //    Debug.Log("speed2 " + speed2);
        //    Debug.Log("weight1 " + weight1);
        //    Debug.Log("weight2 " + weight2);


        //    newSpeed1 = new Vector2((speed1.x * (weight1 - weight2) + 2 * weight2 * speed2.x) / (weight1 + weight2),
        //            (speed1.y * (weight1 - weight2) + 2 * weight2 * speed2.y) / (weight1 + weight2));
        //    newSpeed2 = new Vector2((speed2.x * (weight2 - weight1) + 2 * weight1 * speed1.x) / (weight1 + weight2),
        //            (speed2.y * (weight2 - weight1) + 2 * weight1 * speed1.y) / (weight1 + weight2));
        //    Debug.Log("newSpeed1 " + newSpeed1);
        //    Debug.Log("newSpeed2 " + newSpeed2);
        //}
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (gs != null && (collision.collider.gameObject.layer == enemyLayer || collision.collider.gameObject.layer == staticLayer))
            {
                Vector2 dir = collision.contacts[0].point - (Vector2)gs.transform.position;
                dir = -dir.normalized;
                Debug.Log("dir: " + dir.ToString());
                Debug.Log("getVelocity: " + gs.getVelocity().ToString());
                Debug.Log("Reflect: " + Vector2.Reflect(gs.getVelocity(), dir));
                Debug.Log("Reflect2: " + gs.getVelocity() * dir);

                gs.getRigedBody().AddForce(Vector2.Reflect(gs.getVelocity(), dir));
            }

            //if (collision.collider.gameObject.layer == enemyLayer)
            //{
            //    Debug.Log("Collidered "+ collision.collider.gameObject.name);
                
            //    GameShip otherShip = collision.collider.gameObject.GetComponentInParent<GameShip>();
            //    Vector2 newSpeed1, newSpeed2;
            //    CalcNewSpeed(gs.getWeight(), gs.getVelocity(), otherShip.getWeight(), otherShip.getVelocity(), out newSpeed1, out newSpeed2);


            //    gs.setVelocity(Bounce(gs.getVelocity(), collision.contacts[0].normal) * newSpeed1.magnitude);
            //    //otherShip.setVelocity(Bounce(otherShip.getVelocity(), collision.contacts[0].normal) * newSpeed2.magnitude);
            //    Debug.Log("newSpeed1.magnitude " + newSpeed1.magnitude);
            //    Debug.Log("gs.positionSpeed " + gs.getVelocity());
            //    Debug.Log("newSpeed2.magnitude " + newSpeed2.magnitude);
            //    Debug.Log("otherShip.positionSpeed " + otherShip.getVelocity());

            //    collision.collider.gameObject.SendMessage("ApplyDamage", 10.0);
            //}
            //else if (collision.collider.gameObject.layer == staticLayer && collision.otherCollider.gameObject.layer != staticLayer)
            //{

            //    Debug.Log("Collidered " + collision.collider.gameObject.name);
            //    gs.setVelocity(Bounce(gs.getVelocity(), collision.contacts[0].normal) * gs.getVelocity().magnitude);


            //    gameObject.SendMessage("ApplyDamage", 10.0);
            //}
        }
    }
}