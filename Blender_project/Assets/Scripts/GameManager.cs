using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera Cam;

    public static GameManager instance;

    public float Time;

    public bool day = true;
    public bool night = false;

    public Color Day_col;
    public Color Night_col;

    public float seconds_in_a_day;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            TICKER.OnTickAction += Tick;
            TICKER.OnTickAction_2 += Tick_2;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this.gameObject);

        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Tick()
    {

    }

    void Tick_2()
    {
        Time += 1;

        if (Time > seconds_in_a_day)
        {
            Time = 0;

            day = !day;
            night = !night;
        }

        if (day)
        {
            Cam.backgroundColor = Color.Lerp(Night_col, Day_col, Time / (seconds_in_a_day / 3));
        }

        if (night)
        {
            Cam.backgroundColor = Color.Lerp(Day_col, Night_col, Time / (seconds_in_a_day / 3));
        }
    }
}
