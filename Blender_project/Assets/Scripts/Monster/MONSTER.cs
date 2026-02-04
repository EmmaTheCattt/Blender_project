using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MONSTER : SPAWNABLE
{
    public string Name;

    public int Level;

    public int Spawn_level_change_amount_low;
    public int Spawn_level_change_amount_high;

    public float hp;
    public float Base_MAXHP;
    
    public float MAX_HP;
    public float EXTRA_HP;

    public float damage;

    public float distance_rend;

    public Collider COL;

    public Slider SLIDE;
    public Image SLIDE_IMAGE;

    public Color Green;
    public Color Yellow;
    public Color Red;

    public TextMeshProUGUI NAMEBOX;

    public GameObject Player;
    public GameObject CAM;

    public Canvas CANVAS;
    public Vector3 look_vec;


    private float time_hp = 0;
    public float time_dead = 0;

    private void OnEnable()
    {
        CAM = GameObject.FindGameObjectWithTag("MainCamera");
        TICKER.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        TICKER.OnTickAction -= Tick;
    }
    void Tick()
    {
        passive();
    }

    public void SET_VAL()
    {
        if (Spawned_from != null)
        {
            Set_LEVEL();
        }

        hp = MAX_HP;
        NAMEBOX.text = Name.ToString() + "\n" + "LV: " + Level.ToString() + "\n" + hp.ToString() + "/" + MAX_HP.ToString();
    }

    public void Set_LEVEL()
    {
        int random_level_change = Random.Range(Spawn_level_change_amount_low, Spawn_level_change_amount_high);

        Level += random_level_change;

        MAX_HP = Base_MAXHP + EXTRA_HP * Level;
    }

    public void passive()
    {
        //time_hp -= Time.deltaTime;

        if (time_hp <= 0)
        {
            //SLIDE.gameObject.SetActive(false);
        }

        if (hp <= 0)
        {
            time_dead += Time.deltaTime;
            die();
        }

        float distance = Vector2.Distance(CAM.transform.position, transform.position);
        if (CANVAS != null && distance < distance_rend)
        {
            CANVAS.enabled = true;
            look_vec = new Vector3(CAM.transform.position.x, this.transform.position.y, CAM.transform.position.z);
            CANVAS.transform.LookAt(look_vec);
        }
        else
        {
            CANVAS.enabled = false;
        }
    }

    void Find_player()
    {

    }

    public void Take_damage(float damage)
    {
        SLIDE.gameObject.SetActive(true);
        SLIDE.maxValue = MAX_HP;
        SLIDE.minValue = 0;

        time_hp = 10;

        hp -= damage;
        SLIDE.value = hp;

        if (hp <= 0)
        {
            time_dead += Time.deltaTime;
            die();
        }

        NAMEBOX.text = Name.ToString() + "\n" + "LV: " + Level.ToString() + "\n" + hp.ToString() + "/" + MAX_HP.ToString();
    }

    public void die()
    {
        NAMEBOX.text = string.Empty;
        SLIDE.gameObject.SetActive(false);

        transform.position += Vector3.down * 5 * Time.deltaTime;

        if (time_dead >= 1)
        {
            vanish();
        }
    }

    public void vanish()
    {
        if (Spawned_from != null)
        {
            Spawned_from.GetComponent<Spawner>().OBJECTS -= 1;
        }

        Destroy(this.gameObject);
    }
}
