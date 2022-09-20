using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace StarBattles{
    public abstract class DamageObject : MonoBehaviour
    {
        Rigidbody2D body;
        public Vector3 positionSpeed;
        public double damage;
        public double maxDistance;
        Vector3 orgPos;
        int enemyLayer;
        int staticLayer;
        bool ending = false;

        void Start()
        {
            orgPos = transform.position;
            enemyLayer = LayerMask.NameToLayer("EnemyShips");
            staticLayer = LayerMask.NameToLayer("StaticObject");
            body = GetComponent<Rigidbody2D>();
            body.velocity = this.positionSpeed;
            maxDistance = 30;
            Begin();
        }

        // Update is called once per frame
        void Update()
        {
            if (!ending)
            {
                //float frameSeconds = (Time.time - lastTime);
                //lastTime = Time.time;
                //this.gameObject.transform.position += this.positionSpeed * frameSeconds;
                if ((transform.position - orgPos).magnitude > maxDistance)
                {
                    StartEnd();
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!ending)
            {
                if (other.gameObject.layer == enemyLayer)
                {
                    StartEnd();
                    other.gameObject.SendMessage("ApplyDamage", damage);
                }
                else if (other.gameObject.layer == staticLayer)
                {
                    StartEnd();
                }
            }
        }
        protected void StartEnd()
        {
            ending = true;
            End();
        }
        protected abstract void Begin();
        protected abstract void End();
    }
}