using UnityEngine;

public class DoDamageToPlayer : MonoBehaviour
{
    public int dmg;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            try
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(dmg);
            }
            catch { }
        }
    }
}
