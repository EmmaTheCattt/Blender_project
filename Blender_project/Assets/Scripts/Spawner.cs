using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject center;

    public GameObject X_GO;
    public GameObject Z_GO;

    public GameObject X_RESTRICT;
    public GameObject Z_RESTRICT;

    public float X_val;
    public float Z_val;

    public float X_RESTRICT_val;
    public float Z_RESTRICT_val;

    public GameObject[] PREFABS;
    public int random_PREFAB;

    public float random_x;
    public float random_z;

    public float RAY_HEIGHT;
    public float Spawn_height;

    public float PREFAB_lim;
    public float OBJECTS = 0f;

    public bool ALL_ON_STARTUP;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        X_val = X_GO.transform.localPosition.x;
        Z_val = Z_GO.transform.localPosition.z;

        X_RESTRICT_val = X_RESTRICT.transform.localPosition.x;
        Z_RESTRICT_val = Z_RESTRICT.transform.localPosition.z;
        
        if (ALL_ON_STARTUP)
        {
            while (OBJECTS < PREFAB_lim)
            {
                SPAWN_OBJECT();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OBJECTS < PREFAB_lim)
        {
            SPAWN_OBJECT();
        }
    }

    private void SPAWN_OBJECT()
    {
        random_x = Random.Range(-X_val, X_val);
        random_z = Random.Range(-Z_val, Z_val);

        random_PREFAB = Random.Range(0, PREFABS.Length);

        if ((random_x < X_RESTRICT_val && random_x > 0) || (random_x > -X_RESTRICT_val && random_x < 0))
        {
            if ((random_z < Z_RESTRICT_val && random_z > 0) || (random_z > -Z_RESTRICT_val && random_z < 0))
            {
                if (random_z > 0)
                {
                    random_z += Z_RESTRICT_val;
                }

                if (random_z < 0)
                {
                    random_z += -Z_RESTRICT_val;
                }
            }
        }

        RaycastHit hit;

        if (Physics.Raycast(new Vector3(random_x, RAY_HEIGHT, random_z), Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("GROUND"))
            {
                GameObject OBJECT = Instantiate(PREFABS[random_PREFAB], new Vector3(random_x, RAY_HEIGHT + Spawn_height - hit.distance, random_z), Quaternion.identity);
                OBJECT.GetComponent<SPAWNABLE>().Spawned_from = this.gameObject;
                OBJECT.transform.rotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
                OBJECTS++;
            }
        }
    }
}
