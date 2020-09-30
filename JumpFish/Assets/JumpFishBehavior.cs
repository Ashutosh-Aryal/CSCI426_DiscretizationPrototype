using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpFishBehavior : MonoBehaviour
{
    private const KeyCode JUMP_UP_KEY = KeyCode.W;
    private const KeyCode JUMP_DOWN_KEY = KeyCode.S;
    private const int RESPAWN_INDEX = 3;

    private const string ADD_POINT_TRIGGER_TAG = "ResetNumJumps";
    private const string WALL_TAG = "Wall";
    private const string NUM_JUMPS_LEFT_TEXT = "Number of Jumps Left: ";

    private const float JUMP_MAX_THRESHOLD_INCREMENT = 0.25f;

    [SerializeField]
    private GameObject m_TeleportLocationsContainer;

    [SerializeField]
    private Slider m_Slider;

    [SerializeField]
    private Text m_NumberOfJumpsLeft;

    public static int s_Score = 0;

    private int m_NumAllowedJumps = 3;
    private int m_CurrentAllowedJumps = 3;

    private float m_JumpHoldingTimer = 0.0f;
    private int m_NumRowsToJump = 0;
    private int m_CurrentLocationIndex;
    private bool m_ShouldIncrementJumpTimer = false;
    
    private List<GameObject> m_TeleportLocations = new List<GameObject>();

    AudioSource audioSrc;

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.gameObject.CompareTag(ADD_POINT_TRIGGER_TAG)) {
            audioSrc.Play();

            s_Score++;

            if(s_Score > 10) {
                m_NumAllowedJumps = 1;
            } else if(s_Score > 5) {
                m_NumAllowedJumps = 2;
            }

            m_CurrentAllowedJumps = m_NumAllowedJumps;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.gameObject.CompareTag(WALL_TAG)) {

            s_Score = 0;
            m_NumAllowedJumps = 3;
            m_CurrentAllowedJumps = m_NumAllowedJumps;
            DisplayEndGameState(); // should restart game and reset player & wall locations
        }
    }

    private void DisplayEndGameState()
    {
        gameObject.transform.position = m_TeleportLocations[RESPAWN_INDEX].transform.position;
        WallMovement.DestroyCurrentWalls();
    }

    // Start is called before the first frame update
    void Start()
    {
        int numChildren = m_TeleportLocationsContainer.transform.childCount;

        for(int x = 0; x < numChildren; x++) {
            m_TeleportLocations.Add(m_TeleportLocationsContainer.transform.GetChild(x).gameObject);
        }

        m_CurrentLocationIndex = GetLocationIndex();

        m_Slider.minValue = 0;
        m_Slider.maxValue = JUMP_MAX_THRESHOLD_INCREMENT * (m_TeleportLocations.Count - 1);
        audioSrc = GetComponent<AudioSource>();

    }

    private int GetLocationIndex()
    {
        int locationIndex = 0;
        float minDistance = 9999.0f;

        for(int x = 0; x < m_TeleportLocations.Count; x++) {

            float distance = (gameObject.transform.position - m_TeleportLocations[x].transform.position).magnitude;

            if(distance < minDistance) {
                locationIndex = x;
                minDistance = distance;
            }
        }

        return locationIndex;
    }

    private void FixedUpdate() {

        float THRESHOLD_TO_INCREMENT_NUM_ROWS_JUMPED = JUMP_MAX_THRESHOLD_INCREMENT * Mathf.Abs(m_NumRowsToJump);
        
        if (m_ShouldIncrementJumpTimer) {
            m_JumpHoldingTimer += Time.fixedDeltaTime;
            m_Slider.value = THRESHOLD_TO_INCREMENT_NUM_ROWS_JUMPED - JUMP_MAX_THRESHOLD_INCREMENT;
        }

        bool shouldIncrementRowsToJump = m_JumpHoldingTimer >= THRESHOLD_TO_INCREMENT_NUM_ROWS_JUMPED;

        if(shouldIncrementRowsToJump) {
            m_NumRowsToJump += (m_NumRowsToJump > 0) ? 1 : -1;
        }
    }

    // Update is called once per frame
    void Update() {

        m_NumberOfJumpsLeft.text = NUM_JUMPS_LEFT_TEXT + m_CurrentAllowedJumps;

        if(m_CurrentAllowedJumps <= 0) {
            return;
        }

        if(Input.GetKeyDown(JUMP_UP_KEY)) {
            m_NumRowsToJump = -1; m_ShouldIncrementJumpTimer = true;
        } else if(Input.GetKeyDown(JUMP_DOWN_KEY)) {
            m_NumRowsToJump = 1; m_ShouldIncrementJumpTimer = true;
        } else if(Input.GetKeyUp(JUMP_UP_KEY) || Input.GetKeyUp(JUMP_DOWN_KEY)) {
            
            int jumpToIndex = m_CurrentLocationIndex + m_NumRowsToJump;
            m_CurrentLocationIndex = Mathf.Clamp(jumpToIndex, 0, m_TeleportLocations.Count - 1);
            gameObject.transform.position = m_TeleportLocations[m_CurrentLocationIndex].transform.position;
            m_JumpHoldingTimer = 0.0f; m_ShouldIncrementJumpTimer = false;
            m_Slider.value = 0.0f;
            m_CurrentAllowedJumps--;
        } 
    }
}
