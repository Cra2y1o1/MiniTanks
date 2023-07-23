using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed;
    public float lifetime;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;
    private NetworkMatch nm;

    private void Start()
    {
        Invoke("DestroyBullet", lifetime);
        nm = GetComponent<NetworkMatch>();
    }

    [ServerCallback]
    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null && hitInfo.collider.GetComponent<NetworkMatch>().matchId ==
            nm.matchId)
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                hitInfo.collider.GetComponent<Player>().TakeDamage(damage);
            }

            DestroyBullet();
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    [Server]
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
