using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarBattles
{
    public class Physics : MonoBehaviour {
        GameShip gs;
        int enemyLayer;
        // Use this for initialization
        void Start() {
            gs = GetComponentInParent<GameShip>();
            enemyLayer = LayerMask.NameToLayer("EnemyShips");
        }

        // Update is called once per frame
        void Update() {

        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collidered");
            if (collision.collider.gameObject.layer == enemyLayer)
            {
                Debug.Log("Collidered");
                GameShip otherShip = collision.collider.gameObject.GetComponentInParent<GameShip>();
                float w1 = gs.getWeight();
                float w2 = otherShip.getWeight();
                float v1 = gs.positionSpeed.x;
                float v2 = otherShip.positionSpeed.x;
                gs.positionSpeed.x = (v1 * (w1 - w2) + 2 * w2 * v2) / (w1 + w2);
                otherShip.positionSpeed.x = (v2 * (w2 - w1) + 2 * w1 * v1) / (w1 + w2);
                v1 = gs.positionSpeed.y;
                v2 = otherShip.positionSpeed.y;
                gs.positionSpeed.y = (v1 * (w1 - w2) + 2 * w2 * v2) / (w1 + w2);
                otherShip.positionSpeed.y = (v2 * (w2 - w1) + 2 * w1 * v1) / (w1 + w2);
                collision.collider.gameObject.SendMessage("ApplyDamage", 10.0);

            }
        }
    }
}