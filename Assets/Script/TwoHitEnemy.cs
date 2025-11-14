using UnityEngine;

public class TwoHitEnemy : Enemy
{
    private bool firstHit = false;

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Block")) return;

        Block block = collision.gameObject.GetComponent<Block>();

        // 1‰ñ–Ú‚ÌÕ“Ë
        if (!firstHit)
        {
            firstHit = true;
            block.StopMove();    // Block‚ÌˆÚ“®‚Í~‚ß‚é
            return;
        }

        // 2‰ñ–Ú‚ÌÕ“Ë ¨ Á‚¦‚é
        block.StopMove();
        Destroy(gameObject);
    }
}
