using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    // =============== Player ==============
    // Transforms of head and Body of the character
    public Transform head;
    public Transform body;
    private Vector3 headInitLocalPosition;
    private Vector3 bodyInitLocalScale;
    private Vector3 playerPosition;

    // Rigidbody of Character
    public Rigidbody rb;

    // force to be added
    private float force = 4;
    // Start time to add a force
    private float startAddForceTime;

    // Particle effect attached on the character
    public GameObject particleMain;
    public GameObject particleLanding;
    public GameObject particleExplosion;

    // Relative Position of Camera
    public Camera mainCamera;
    private Vector3 cameraRelativePosition;

    // Enable player to input or not
    private bool enableInput = false;

    // The previous obtained reward. We may double this reward if perform perfectly
    private int reward = 1;

    //public GameObject GameOver;
    public bool isGameOver;

    // reference other classes
    public StageManager stageManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    public bool isJuicy = false;

    // Start is called before the first frame update
    void Start()
    {
        // For animation shape recovery, save the initial values
        headInitLocalPosition = head.transform.localPosition;
        bodyInitLocalScale = body.transform.localScale;

        // Direction Vector of the character
        cameraRelativePosition = mainCamera.transform.position - this.transform.position;

        ///////////////// Set the center of mass for rigidbody
        rb.centerOfMass = Vector3.zero;

        // game is alive
        isGameOver = false;
        
        
    }

    void Update()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        rb.WakeUp(); // Avoid the rigidbody to sleep
        if (enableInput)
        {
            // When click the left button of mouse
            if (Input.GetMouseButtonDown(0))
            {
                ///////////////// Start to charge the force
                startAddForceTime = Time.time;
                if (isJuicy)
                {
                    // activate the particle effect
                    particleMain.SetActive(true);
                    particleLanding.SetActive(false);

                    // play audio clip for "add force"
                    audioManager.PlayAudio(enumAudioClip.AddForce);
                }
                
            }
            // When press the left button of the mouse,
            // squeeze down the stage, and scale down the y axis of character
            if (Input.GetMouseButton(0))
            {
                // Give a boundary of scale down
                if (stageManager.currentStage.transform.localScale.y > 0.4f)
                {
                    // Shrink the body
                    body.transform.localScale += new Vector3(1.5f, -1f, 1.5f) * 0.05f * Time.deltaTime;
                    // Move head down along with the scale down of body
                    head.transform.localPosition += new Vector3(0, -1, 0) * 0.1f * Time.deltaTime;
                    // Change the stage scale and position due to shrink
                    stageManager.currentStage.transform.localScale += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
                    stageManager.currentStage.transform.localPosition += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
                }else if (stageManager.currentStage.transform.localScale.y <= 0.4f)
                {
                    OverCharging();
                }
            }
            // When release the button
            if (Input.GetMouseButtonUp(0))
            {
                /////////////// forbid to continuous jump
                enableInput = false;
                // Turn off the audio component
                //GetComponent<AudioSource>().Stop();
                audioManager.audioSource.Stop();
                ///////////////// Obtain how long it charges for force
                var elapse = Time.time - startAddForceTime;
                if (elapse > 1)
                {
                    // Maximum time for charging of force is 1.
                    elapse = 1;
                }

                if (isJuicy)
                {
                    // Deactivate particle
                    particleMain.SetActive(false);
                    audioManager.PlayAudio(enumAudioClip.Jump);
                }
                
                //////////////////////// Jump
                OnJump(elapse);

                // Recover character shape (using DOTween)
                body.transform.DOScale(new Vector3(bodyInitLocalScale.x, bodyInitLocalScale.y, bodyInitLocalScale.z), 0.2f);
                // The original local position for y is 0.29f
                head.transform.DOLocalMoveY(headInitLocalPosition.y, 0.2f);

                // Recover the shape of the stage
                stageManager.currentStage.transform.DOLocalMoveY(stageManager.stageInitPosition.y, 0.4f);
                stageManager.currentStage.transform.DOScaleY(stageManager.stageInitScale.y, 0.4f);

            }
        }
    }

    private void OnJump(float elapse)
    {
        /////////////// Seek jump direction by subtracting character position from next stage position
        Vector3 jumpDirection = (stageManager.nextStage.transform.position - this.transform.position).normalized;
        jumpDirection.y = 0;

        rb.AddForce(new Vector3(0, 5f, 0) + jumpDirection * elapse * force, ForceMode.Impulse);

        if (jumpDirection.x == 1)
        {
            // Front flip around the z-axis
            transform.DOLocalRotate(new Vector3(0, 0, -360), 0.6f, RotateMode.LocalAxisAdd);
        }
        else
        {
            // Front flip around the x - axis
            transform.DOLocalRotate(new Vector3(-360, 0, 0), 0.6f, RotateMode.LocalAxisAdd);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            audioManager.PlayAudio(enumAudioClip.GetCoin);
        }
    }
    
    // Collision event with ground
    private void OnCollisionEnter(Collision collision)
    {
        rb.Sleep(); // sleep one frame after collision to avoid the swing

        //if (GameOver.activeInHierarchy)
        if (isGameOver == true)
        {
            return;
        }
        if (collision.transform.CompareTag("Stage") == false)
        {
            // If the collision object is not a stage, game over.
            PlayerDie();
            return;
        }

        if (stageManager.currentStage != collision.gameObject)
        {
            stageManager.currentStage = collision.gameObject;
            // Spawn new stage
            stageManager.SpawnStage();

            // Move particle
            playerPosition = GameObject.Find("Player").transform.position;
            particleExplosion.transform.localPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
            
            // Move Camera
            MoveCamera();

            // Add score
            AddScore();
            // Enable input for player
            enableInput = true;
            if (isJuicy)
            {
                // play audio clip
                audioManager.PlayAudio(enumAudioClip.Success);
                // activate landing particle
                particleLanding.SetActive(true);
            }
            

        }
        else // if collision object is current stage, then enable input
        {
            enableInput = true; // Default to be true for enabling input
        }
    }

    // Add score
    private void AddScore()
    {
        // obtain the character position
        Vector3 hitPos = this.transform.position;
        hitPos.y = stageManager.currentStage.transform.position.y;
        // calculate the distance with the target
        float targetDistance = Vector3.Distance(hitPos, stageManager.currentStage.transform.position);
        if (targetDistance < 0.1f)
        {
            // Double the score if very close to the center
            reward *= 2;
        }
        else
        {
            reward = 1;
        }
        uiManager.OnAddScore(reward);
    }

    /// <summary>
    /// Handle character death
    /// </summary>
    private void PlayerDie()
    {
        enableInput = false; // ??????????????????????

        uiManager.OnGameOver();
        if (isJuicy)
        {
            // play audio clip
            audioManager.PlayAudio(enumAudioClip.Fall);
        }
        
        isGameOver = true;
    }

    private void OverCharging()
    {
        enableInput = false;
        
        uiManager.OnGameOver();
        if (isJuicy)
        {
            Vector3 exforce = new Vector3(Random.Range(-3.0f, 3.0f), 
                Random.Range(5.0f, 15.0f),
                Random.Range(-3.0f, 3.0f)); 
            particleExplosion.SetActive(true);
            particleMain.SetActive(false);
            audioManager.audioSource.Stop();
            audioManager.PlayAudio((enumAudioClip.Explosion));
            
            rb.AddForce(exforce, ForceMode.Impulse);
        }

        isGameOver = true;
    }
    /// <summary>
    /// Move camera to follow the character
    /// </summary>
    private void MoveCamera()
    {
        // shake camera
        mainCamera.transform.DOShakePosition(0.25f, 0.075f);
        StartCoroutine(CameraEffect());
    }

    private IEnumerator CameraEffect()
    {
        yield return new WaitForSeconds(0.26f);
        //Camera.main.transform.DOMove(this.transform.position + CameraRelativePosition, 1);
        mainCamera.transform.DOMove(this.transform.position + cameraRelativePosition, 1);
    }
    
}
