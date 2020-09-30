using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    private const float X_VELOCITY = -100.0f;
    private const float TIME_BETWEEN_SPAWNS = 5.0f;
    private const float LEFT_X_BOUNDARY = -220.0f;
    
    private static bool s_ShouldSpawnWall = true;

    private float m_SpawnTimer = 0.0f;

    [SerializeField]
    private GameObject m_WallsContainer;

    [SerializeField]
    private Transform m_WallSpawnLocation;

    private static List<GameObject> s_Walls = new List<GameObject>();
    private static List<GameObject> s_MovingWalls = new List<GameObject>();

    public static void DestroyCurrentWalls()
    {
        foreach(GameObject wallOnScreen in s_MovingWalls) {
            Destroy(wallOnScreen);
        }

        s_MovingWalls.Clear();

        s_ShouldSpawnWall = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        int numChildren = m_WallsContainer.transform.childCount;

        for(int x = 0; x < numChildren; x++) {
            s_Walls.Add(m_WallsContainer.transform.GetChild(x).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(s_ShouldSpawnWall) {
            SpawnWall();
        }

        CheckIfOutOfBounds();

        m_SpawnTimer += Time.deltaTime;
    }

    private void SpawnWall()
    {
        int randIndex = Random.Range(0, s_Walls.Count);

        GameObject spawnedWall = Instantiate(s_Walls[randIndex], m_WallSpawnLocation);

        float new_X_Velocity = (X_VELOCITY / 5.0f) * (JumpFishBehavior.s_Score / 10) + X_VELOCITY;

        spawnedWall.GetComponent<Rigidbody2D>().velocity = new Vector2(new_X_Velocity, 0.0f);

        s_MovingWalls.Add(spawnedWall);

        s_ShouldSpawnWall = false;
    }

    private void CheckIfOutOfBounds()
    {
        List<int> removedIndices = new List<int>();
        
        for(int x = 0; x < s_MovingWalls.Count; x++)
        {
            if(s_MovingWalls[x].transform.position.x <= LEFT_X_BOUNDARY)
            {
                removedIndices.Add(x);
                s_ShouldSpawnWall = true;
            }
        }

        foreach(int index in removedIndices)
        {
            Destroy(s_MovingWalls[index]);
            s_MovingWalls.RemoveAt(index);
        }

    }
}
