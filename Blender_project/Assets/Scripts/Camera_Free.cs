using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Camera_Free : MonoBehaviour
{

    public GameObject Target_direction;
    public GameObject Side_direction;
    public float speed = 10f;
    public float Fast_speed = 20f;

    Vector3 Movement;
    Vector3 Add_vector;

    public float SensX;
    public float SensY;

    public float rotationX;
    public float rotationY;

    public bool forward;
    public bool Back;
    public bool Left;
    public bool Right;
    public bool Shift;
    public bool Moving;

    //LOCKED OR NOT LOCKED TO PLAYER
    public bool LOCKED = true;
    public GameObject Player_point;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        Back    = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        Left    = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        Right   = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        Moving = forward || Back || Left || Right;
        Shift   = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Cam_ROTATION();
    }

    private void FixedUpdate()
    {
        if (!LOCKED)
        {
            FreeCam();
        }

        if (LOCKED)
        {
            Player_locked();
        }
    }

    private void Player_locked()
    {
        transform.position = Player_point.transform.position;
    }

    private void FreeCam()
    {
        if (forward || Back)
        {
            if (forward)
            {
                Add_vector = (Target_direction.transform.position - transform.position);
                Add_vector = Add_vector.normalized;
                Movement += Add_vector;
            }

            if (Back)
            {
                Add_vector = (transform.position - Target_direction.transform.position);
                Add_vector = Add_vector.normalized;
                Movement += Add_vector;
            }
        }


        if (Left || Right)
        {
            if (Left)
            {
                Add_vector = (transform.position - Side_direction.transform.position);
                Add_vector = Add_vector.normalized;
                Movement += Add_vector;
            }

            if (Right)
            {
                Add_vector = (Side_direction.transform.position - transform.position);
                Add_vector = Add_vector.normalized;
                Movement += Add_vector;
            }
        }


        if (!Moving)
        {
            Movement = Vector3.zero;
            Add_vector = Vector3.zero;
        }

        if (Shift)
        {
            transform.position += Movement * Fast_speed * Time.fixedDeltaTime;
        }
        else
        {
            transform.position += Movement * speed * Time.fixedDeltaTime;
        }

        Movement = Vector3.zero;
    }

    private void Cam_ROTATION()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SensY;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        //Target_direction.transform.rotation = quaternion.Euler(rotationX, rotationY, 0);
    }
}
