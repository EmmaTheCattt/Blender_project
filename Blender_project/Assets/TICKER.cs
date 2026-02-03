using UnityEngine;

public class TICKER : MonoBehaviour
{
    public static float TickTime = 0.1f;

    private float TIMER;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    public void Update()
    {
        TIMER += Time.deltaTime;

        if (TIMER >= TickTime)
        {
            TIMER = 0;
            TickEvent();
        }
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }
}
