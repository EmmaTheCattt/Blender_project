using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WEAPON : MonoBehaviour
{
    public Player_movement player;

    public string name;

    public float base_damage;
    public float damage = 0; 

    public float extra_damage = 0;
    public float charge_add;
    public float max_damage;

    public float base_dash_speed;
    public float dev_dash;

    public float min_dash_speed;
    public float max_dash_speed;
    public float fall_off;

    public bool melee;
    public bool ranged;

    public Collider HITBOX;
    public AudioSource AUD;
    public AudioClip Attack_CLIP;

    public void passive()
    {
        player.max_Damage = max_damage;
        player.extra_damage = extra_damage;

        if (player.swing && !player.Shift)
        {
            extra_damage += charge_add * Time.deltaTime;

            if (extra_damage >= max_damage)
            {
                extra_damage = max_damage;
            }

            damage = base_damage + extra_damage;
            player.DASH_SPEED = base_dash_speed + extra_damage / (max_damage / (max_dash_speed - base_dash_speed));

            if (player.DASH_SPEED >= max_dash_speed)
            {
                player.DASH_SPEED = max_dash_speed;
            }

            if (player.DASH_SPEED < min_dash_speed)
            {
                player.DASH_SPEED = 0;
            }
            
            dev_dash = player.DASH_SPEED;
        }

        if (player.charged_swing)
        {
            no_extra_attack();
        }

        if (player.dashing)
        {
            player.DASH_SPEED -= fall_off * Time.deltaTime;

            if (player.DASH_SPEED <= 0)
            {
                player.DASH_SPEED = 0;
                player.dashing = false;
                player.dashed = true;
            }
        }
    }

    public void no_extra_attack()
    {
        extra_damage = 0;
    }

    public void Attack_sound()
    {
        AUD.clip = Attack_CLIP;
        AUD.Play();
    }

    private void OnTriggerEnter(Collider col)
    {
        damage = Mathf.Round(damage);

        if (col.CompareTag("MONSTER"))
        {
            col.GetComponent<MONSTER>().Take_damage(damage);
        }
    }
}