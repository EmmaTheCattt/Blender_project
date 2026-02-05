using UnityEngine;

public class TICKER : MonoBehaviour
{
    public static float TickTime = 0.1f;

    public static float TickTime2 = 1f;

    private float TIMER;
    private float TIMER2;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    public delegate void TickAction_2();
    public static event TickAction_2 OnTickAction_2;

    public void Update()
    {
        TIMER += Time.deltaTime;
        TIMER2 += Time.deltaTime;

        if (TIMER >= TickTime)
        {
            TIMER = 0;
            TickEvent();
        }

        if (TIMER2 >= TickTime2)
        {
            TIMER2 = 0;
            TickEvent_2();
        }
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }

    private void TickEvent_2()
    {
        OnTickAction_2?.Invoke();
    }
}
