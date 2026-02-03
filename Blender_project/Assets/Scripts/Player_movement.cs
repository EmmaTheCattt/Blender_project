using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Player_movement : MonoBehaviour
{
    public GameObject Cam;
    public GameObject CAM_SPOT;
    public GameObject Center;
    public GameObject Body;
    public GameObject Helmet;
    public GameObject Head_space;
    public GameObject GM_PLAYER;
    public Camera_Free Cam_script;

    public Slider Charge;

    //UI
    public TextMeshProUGUI Cam_info;
    public TextMeshProUGUI Dev_info;

    public bool Cam_button;
    public bool Dev_info_button;
    public bool hide_info = false;
    public Image Cursor;

    //Cam UI
    public float time;
    public float show_time;
    public float alpha = 1;
    public Color UI_COL = new Color(255, 255, 255, 255);

    //Inputs
    public bool forward;
    public bool Back;
    public bool Left;
    public bool Right;
    public bool jump;
    public bool jump_release;
    public bool swing;
    public bool swing_release;

    public bool Shift;
    public bool Moving;

    //movement
    public Vector3 Movement;
    public Vector3 direction;
    public Vector3 Jump_vec;
    public Vector3 UP_OR_DOWN;
    public Vector3 Dash;
    public Vector3 Dash_direction;

    public float DASH_SPEED;
    public bool dashing = false;
    public float Dashing_time;
    public float Dash_duration;
    public bool dashed = false;

    Vector3 FRONT_MOVEMENT;
    Vector3 SIDE_MOVEMENT;
    public bool FRONT;
    public bool SIDE;

    public float charge_speed;
    public float Speed;
    public float Sprint_speed;
    public float New_speed;

    public float Jump_force;
    public float new_Jump_force;
    public float Jump_amount = 1;
    public float Jump_falloff = 0.1f;
    public float off_ground_time = 0;
    public float Time_to_job = 0.5f;
    public bool charged_Jump;
    public float charge_amount = 0.1f;
    public float Max_jump = 100f;

    public LayerMask OBJECTS;
    public float height = 10f;
    public bool grounded = true;

    Vector3 falling_vec;
    public float fall_speed;
    public float new_fall_speed;
    public float added_fall;
    public float new_added_fall;

    public Vector3 Position_vector;
    public Vector3 SHOREST_vector;
    public float SHORTEST;
    private float hit_dist;

    public Animator Sword;
    const string SWING = "SWORD_SWINGING";
    const string IDLE = "IDLE";
    const string CHARGE = "CHARGE";
    const string CHARGING = "CHARGE_HOLD";
    public Animator Bow;

    public string current_ani = IDLE;
    public bool charged_swing = false;
    public float swing_coldown = 1f;
    public float swing_recharge = 0f;

    public float base_D;
    public float extra_damage;
    public float max_Damage;

    public float respawn_height;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");
        Cam_script = Cam.GetComponent<Camera_Free>();
        GM_PLAYER = GameObject.FindGameObjectWithTag("GM");

        Cam_script.Player_point = CAM_SPOT;
        new_fall_speed = fall_speed;
        new_added_fall = added_fall;
        new_Jump_force = Jump_force;
        swing_recharge = swing_coldown;
        falling_vec = Vector3.down * new_fall_speed * Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //add time
        time += Time.deltaTime;

        if (grounded == false)
        {
            off_ground_time += Time.deltaTime;
        }

        if (jump_release || (Shift && jump))
        {
            charged_Jump = true;
        }

        swing_recharge += Time.deltaTime;
        if (swing_release || (Shift && swing))
        {
            charged_swing = true;
        }

        if (this.transform.position.y < respawn_height)
        {
            transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
            falling_vec = Vector3.zero;
        }

        Rotate_player(Cam_script.LOCKED);
        Player_inputs(Cam_script.LOCKED);
        UI_PART();
    }

    private void UI_PART()
    {
        DEVINFO();
        UI_PART_CAM();
        UI_PART_CHARGE();
    }

    private void DEVINFO()
    {
        if (Dev_info_button == true)
        {
            hide_info = !hide_info;
        }

        if (hide_info == false)
        {
            Dev_info.text = "PLAYER INFO:" + "\n" + "\n"; //TOP UI
            Dev_info.text = Dev_info.text + "Free Cam: " + (!Cam_script.LOCKED).ToString() + "\n";
            Dev_info.text = Dev_info.text + "Grounded: " + grounded.ToString() + "\n";
            Dev_info.text = Dev_info.text + "Fall Speed: " + (Mathf.Round((Jump_vec.magnitude - falling_vec.magnitude) * 10)).ToString() + "\n";
            Dev_info.text = Dev_info.text + "Up Velocity: " + (Mathf.Round((Jump_vec.magnitude) * 10)).ToString() + "\n";
            Dev_info.text = Dev_info.text + "Down Velocity: " + (Mathf.Round((falling_vec.magnitude) * 10)).ToString() + "\n";
            Dev_info.text = Dev_info.text + "Charging: " + (jump && !Shift).ToString() + "\n";
            Dev_info.text = Dev_info.text + "Jump Force: " + new_Jump_force.ToString() + "\n";
            Dev_info.text = Dev_info.text + "\n";
            Dev_info.text = Dev_info.text + "CORDS:" + "\n";
            Dev_info.text = Dev_info.text + "X: " + transform.position.x.ToString() + "\n";
            Dev_info.text = Dev_info.text + "Y: " + transform.position.y.ToString() + "\n";
            Dev_info.text = Dev_info.text + "Z: " + transform.position.z.ToString() + "\n";
            Dev_info.text = Dev_info.text + "\n";
            Dev_info.text = Dev_info.text + "TO HIDE THIS MENU PRESS F1" + "\n";
        }

        if (hide_info == true)
        {
            Dev_info.text = string.Empty;
        }
    }

    private void UI_PART_CHARGE()
    {
        Charge.maxValue = Max_jump;
        Charge.minValue = Jump_force;
        Charge.value = new_Jump_force;

        if (swing)
        {
            Charge.maxValue = max_Damage;
            Charge.minValue = 0;
            Charge.value = extra_damage;

            new_Jump_force = Jump_force;
        }
    }

    private void UI_PART_CAM()
    {
        if (Cam_button)
        {
            time = 0;
            alpha = 1;
            Cam_script.LOCKED = !Cam_script.LOCKED;
            CAM_UI();
        }

        if (time > show_time)
        {
            Cam_info.text = string.Empty;
        }

        alpha -= Time.deltaTime / 3;
        UI_COL = new Color(255, 255, 255, alpha);
        Cam_info.color = UI_COL;

        Cursor.enabled = Cam_script.LOCKED;
    }

    private void Player_inputs(bool Cam_on_player)
    {
        
        forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        Back = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        Left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        Right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        jump = Input.GetKey(KeyCode.Space);

        Moving = forward || Back || Left || Right || jump || dashing;
        Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (swing_coldown < swing_recharge)
        {
            swing = Input.GetMouseButton(0);
            swing_release = Input.GetMouseButtonUp(0);
        }
        else
        {
            swing = false;
            swing_release = false;
        }

        Cam_button = Input.GetKeyDown(KeyCode.C);
        Dev_info_button = Input.GetKeyDown(KeyCode.F1);

        if (Cam_on_player)
        {
            jump_release = Input.GetKeyUp(KeyCode.Space);
        }
    }

    private void FixedUpdate()
    {
        DIRECTION_SET();
        Check_ground();
        Player_move(Cam_script.LOCKED);
        Player_Attack(Cam_script.LOCKED);
    }

    private void DIRECTION_SET()
    {
        direction = new Vector3(CAM_SPOT.transform.position.x - transform.position.x, 0, CAM_SPOT.transform.position.z - transform.position.z);
        direction = direction.normalized;
    }

    void Player_Attack(bool Cam_on_player)
    {
        if (swing && !Shift)
        {
            Animation_change(CHARGE);
            New_speed = charge_speed;

            if (!grounded)
            {
                New_speed = Speed;
            }
        }

        if (charged_swing)
        {
            Animation_change(SWING);
            swing_recharge = 0f;
            New_speed = Speed;
            charged_swing = false;

            if (!dashed)
            {
                Dashing_time = 0;
                dashing = true;

                Dash_direction = Cam.transform.forward;
                Dash_direction = Dash_direction.normalized;

                Movement = Dash_direction * DASH_SPEED;
                Dash = Movement;
                CHECK_WALLS();
            }
        }

        if (dashing)
        {
            Dashing_time += Time.fixedDeltaTime;

            if (Dashing_time > Dash_duration)
            {
                dashing = false;
                dashed = true;
            }

            Movement = Dash_direction * DASH_SPEED;
            Dash = Movement;
            CHECK_WALLS();
        }
    }

    void Animation_change(string New_ani)
    {
        if (current_ani == New_ani)
        {
            return;
        }
        else
        {
            current_ani = New_ani;
        }

        Sword.Play(New_ani);

        if (current_ani == SWING)
        {
            current_ani = IDLE;
        }
    }

    void Check_ground()
    {
        if (Jump_vec != Vector3.zero || grounded == false || Moving == true)
        {
            falling_vec = Vector3.down * new_fall_speed;

            RaycastHit hit;

            SHORTEST = 100f;
            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 0:
                        Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                        SHOREST_vector = Position_vector; break;
                    case 1:
                        Position_vector = new Vector3(transform.position.x + 0.60f, transform.position.y, transform.position.z); break;
                    case 2:
                        Position_vector = new Vector3(transform.position.x - 0.60f, transform.position.y, transform.position.z); break;
                    case 3:
                        Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.60f); break;
                    case 4:
                        Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.60f); break;
                    case 5:
                        Position_vector = new Vector3(transform.position.x + 0.30f, transform.position.y, transform.position.z); break;
                    case 6:
                        Position_vector = new Vector3(transform.position.x - 0.30f, transform.position.y, transform.position.z); break;
                    case 8:
                        Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.30f); break;
                    case 9:
                        Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.30f); break;


                }

                if (Jump_vec.magnitude < falling_vec.magnitude)
                {
                    if (Physics.Raycast(Position_vector, Vector3.down * height, out hit, OBJECTS))
                    {
                        if (hit.distance < SHORTEST)
                        {
                            SHORTEST = hit.distance;
                            SHOREST_vector = Position_vector;
                            UP_OR_DOWN = Vector3.down;
                            continue;
                        }
                    }
                }

                if (Jump_vec.magnitude > falling_vec.magnitude)
                {
                    if (Physics.Raycast(Position_vector, Vector3.up * height, out hit, OBJECTS))
                    {
                        if (hit.distance < SHORTEST)
                        {
                            SHORTEST = hit.distance;
                            SHOREST_vector = Position_vector;
                            UP_OR_DOWN = Vector3.up;
                            continue;
                        }
                    }
                }
            }

            if (Physics.Raycast(SHOREST_vector, UP_OR_DOWN * height, out hit, OBJECTS))
            {
                Debug.DrawRay(SHOREST_vector, UP_OR_DOWN * height, Color.yellow);
                Debug.Log(hit.distance);

                if (height + 0.05 < hit.distance || UP_OR_DOWN == Vector3.up)
                {
                    FALL();
                }

                if (height + 0.1f > hit.distance)
                {
                    if (UP_OR_DOWN == Vector3.down)
                    {
                        HITGROUND_VALUE_CHANGE();
                    }

                    if ((Jump_vec.magnitude <= falling_vec.magnitude || UP_OR_DOWN == Vector3.up) && hit.collider.tag != "WEAPON")
                    {
                        Jump_vec = Vector3.zero;
                    }
                }

                if (height > hit.distance)
                {
                    if (UP_OR_DOWN == Vector3.down)
                    {
                        HITGROUND_VALUE_CHANGE();
                        transform.position += Vector3.up * (height - hit.distance);
                    }

                    if ((Jump_vec.magnitude <= falling_vec.magnitude || UP_OR_DOWN == Vector3.up) && hit.collider.tag != "WEAPON")
                    {
                        Jump_vec = Vector3.zero;
                    }
                }
            }
            else
            {
                FALL();
            }
        }
    }

    private void HITGROUND_VALUE_CHANGE()
    {
        grounded = true;
        new_fall_speed = 0;
        new_added_fall = added_fall;
        off_ground_time = 0f;
        dashed = false;

        Dash.y = 0f;
    }

    private void FALL()
    {
        if (!dashing)
        {
            falling_vec = Vector3.down * new_fall_speed;
            new_fall_speed += new_added_fall * Time.fixedDeltaTime;
            new_added_fall += added_fall * Time.fixedDeltaTime;

            //If things gets weird with falling and jumping this might be the reason!
            transform.position += falling_vec * Time.fixedDeltaTime;
        }

        if (dashing)
        {
            new_fall_speed = 0;
            new_added_fall = added_fall;
            Jump_vec = Vector3.zero;
        }

        grounded = false;
    }

    void Player_move(bool Cam_on_player)
    {
        FRONT_MOVEMENT = Vector3.zero;
        SIDE_MOVEMENT = Vector3.zero;
        FRONT = false;
        SIDE = false;

        if (Cam_on_player && Moving)
        {
            if (Shift)
            {
                New_speed = Sprint_speed;
                new_Jump_force = Jump_force;
            }

            if (forward || Back)
            {
                FRONT = true;

                if (forward)
                {
                    Movement = direction * New_speed;
                    FRONT_MOVEMENT = Movement;
                    CHECK_WALLS();
                }

                if (Back)
                {
                    Movement = -direction * New_speed;
                    FRONT_MOVEMENT = Movement;
                    CHECK_WALLS();
                }

                if (forward && Back)
                {
                    FRONT_MOVEMENT = Vector3.zero;
                }
            }

            if (Left || Right)
            {
                SIDE = true;

                if (Left)
                {
                    Movement = -direction * New_speed;
                    Movement = Quaternion.Euler(0, 90, 0) * Movement;
                    SIDE_MOVEMENT = Movement;
                    CHECK_WALLS();
                }

                if (Right)
                {
                    Movement = direction * New_speed;
                    Movement = Quaternion.Euler(0, 90, 0) * Movement;
                    SIDE_MOVEMENT = Movement;
                    CHECK_WALLS();
                }

                if (Right && Left)
                {
                    SIDE_MOVEMENT = Vector3.zero;
                }
            }

            if (jump && (grounded || Time_to_job > off_ground_time) && !Shift && !swing)
            {
                new_Jump_force += charge_amount * Time.fixedDeltaTime;
                New_speed = charge_speed;

                if (new_Jump_force > Max_jump)
                {
                    new_Jump_force = Max_jump;
                }
            }

            if (Time_to_job < off_ground_time)
            {
                charged_Jump = false;
                new_Jump_force = Jump_force;
            }
        }

        if (charged_Jump && (grounded || Time_to_job > off_ground_time))
        {
            Jump_vec = Vector3.up * new_Jump_force;
            new_Jump_force = Jump_force;
            New_speed = Speed;
            charged_Jump = false;
        }

        //THE ACTUAL MOVING PART!!!
        if (!dashing)
        {
            transform.position += Jump_vec * Time.fixedDeltaTime;
            transform.position += (FRONT_MOVEMENT + SIDE_MOVEMENT).normalized * New_speed * Time.fixedDeltaTime;
        }

        if (dashing)
        {
            transform.position += Dash * Time.fixedDeltaTime;
        }
        
        New_speed = Speed;
    }

    void CHECK_WALLS()
    {
        RaycastHit hit;
        Vector3 Line = Movement;

        for (int i = 0; i < 11; i++) 
        {
            switch (i)
            {
                case 0: 
                    Line = Movement; break;
                case 1:
                    Line = Quaternion.Euler(0, 5, 0) * Movement; break;
                case 2:
                    Line = Quaternion.Euler(0, -5, 0) * Movement; break;
                case 3:
                    Line = Quaternion.Euler(0, 10, 0) * Movement; break;
                case 4:
                    Line = Quaternion.Euler(0, -10, 0) * Movement; break;
                case 5:
                    Line = Quaternion.Euler(0, 15, 0) * Movement; break;
                case 6:
                    Line = Quaternion.Euler(0, -15, 0) * Movement; break;
                case 7:
                    Line = Quaternion.Euler(0, 20, 0) * Movement; break;
                case 8:
                    Line = Quaternion.Euler(0, -20, 0) * Movement; break;
                case 9:
                    Line = Quaternion.Euler(0, 25, 0) * Movement; break;
                case 10:
                    Line = Quaternion.Euler(0, -25, 0) * Movement; break;
            }

            Position_vector = new Vector3(transform.position.x, transform.position.y + 0.80f, transform.position.z);
            if (Physics.Raycast(Position_vector, Line, out hit, OBJECTS))
            {
                hit_dist = 2f;

                if (dashing)
                {
                    hit_dist = 2.25f;
                }

                CHECK_DISTANCE(hit);

                if (hit.distance < hit_dist && hit.collider.tag != "WEAPON")
                {
                    break;
                }
            }

            Position_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (Physics.Raycast(Position_vector, Line, out hit, OBJECTS))
            {
                hit_dist = 1.25f;

                if (dashing)
                {
                    hit_dist = 1.5f;
                }

                CHECK_DISTANCE(hit);

                if (hit.distance < hit_dist && hit.collider.tag != "WEAPON")
                {
                    break;
                }
            }

            Position_vector = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
            if (Physics.Raycast(Position_vector, Line, out hit, OBJECTS))
            {
                hit_dist = 0.275f;

                if (dashing)
                {
                    hit_dist = 0.35f;
                }
                
                CHECK_DISTANCE(hit);

                if (hit.distance < hit_dist && hit.collider.tag != "WEAPON")
                {
                    break;
                }
            }
        }
    }

    private void CHECK_DISTANCE(RaycastHit hit)
    {
        if (hit.distance < hit_dist && hit.collider.tag != "WEAPON")
        {
            Movement = Vector3.zero;

            if (SIDE)
            {
                SIDE_MOVEMENT = Vector3.zero;

                //ULTRA SPAGEHETTI CODE. I have no idea why this works for both the front and side directions.
                if (FRONT)
                {
                    FRONT = false;
                }
            }

            if (FRONT)
            {
                FRONT_MOVEMENT = Vector3.zero;
            }

            if (dashing)
            {
                Dash = Vector3.zero;
                dashing = false;
            }
        }
    }

    void CAM_UI()
    {
        if (Cam_script.LOCKED == true)
        {
            Cam_info.text = "FREE CAM: LOCKED";
        }

        if (Cam_script.LOCKED == false)
        {
            Cam_info.text = "FREE CAM: UNLOCKED";
        }
    }

    void Rotate_player(bool Cam_on_player)
    {
        if (Cam_on_player)
        {
            //WE ARE IN THE WILD WEST WITH THIS ONE!!! YEEEHAAAAA!!!
            //Cam.transform.position = Center.transform.position;
            float new_rotationX = Mathf.Clamp(Cam_script.rotationX, -30f, 30f);
            transform.rotation = Quaternion.Euler(new_rotationX, Cam_script.rotationY, 0);
            transform.Rotate(0, 90, 0);
            Cam.transform.position = CAM_SPOT.transform.position;
            Body.transform.rotation = Quaternion.Euler(0, 0, 0);

            //HELMET STUFF
            Helmet.transform.position = Head_space.transform.position;
            Helmet.transform.LookAt(CAM_SPOT.transform.position);
            Helmet.transform.Rotate(0, 90, 0);
        }
    }
}
