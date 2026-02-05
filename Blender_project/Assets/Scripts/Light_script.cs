using UnityEngine;

public class Light_script : MonoBehaviour
{

    public Light lighting;
    public GameManager GM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lighting = GetComponent<Light>();
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();

        TICKER.OnTickAction_2 += Tick;
    }

    void Tick()
    {
        if (GM.day)
        {
            lighting.intensity += (3 / (GM.seconds_in_a_day / 3));

            if (lighting.intensity > 3)
            {
                lighting.intensity = 3;
            }
        }

        if (GM.night)
        {
            lighting.intensity -= (3 / (GM.seconds_in_a_day / 3));

            if (lighting.intensity < 0)
            {
                lighting.intensity = 0;
            }
        }
    }
}
