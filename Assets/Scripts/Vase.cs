using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Vase : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] shardPrefabs; // Assign your 6 shard prefabs here
    public float shatterSpeed = 3f;
    public float shardLifetime = 1.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryBreak(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        TryBreak(collider.gameObject);
    }

    void TryBreak(GameObject other)
    {
        var projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            Break();
        }
    }

    public void Break()
    {
        foreach (GameObject shardPrefab in shardPrefabs)
        {
            // Spawn shard at the vase's current position
            GameObject shardObj = Instantiate(shardPrefab, transform.position, Quaternion.identity);
            
            // Get the script on the shard
            ShardController shardScript = shardObj.GetComponent<ShardController>();

            if (shardScript != null)
            {
                // Generate a random direction in 360 degrees
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                shardScript.Initialize(randomDir, shatterSpeed, shardLifetime);
            }
        }

        // Hide or Destroy the original vase
        Destroy(gameObject);
    }

    // Context menu allows you to test it by right-clicking the component in Inspector
    [ContextMenu("Test Break")]
    private void TestBreak() => Break();
}