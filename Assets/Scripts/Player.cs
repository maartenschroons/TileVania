using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    CapsuleCollider2D myBodycollider;
    BoxCollider2D myFeet;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    Animator myanimator;
    bool isAlive = true;
    float gravityScaleAtStart;
    Vector2 deathKick = new Vector2(25f, 25f);
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myanimator = GetComponent<Animator>();
        myBodycollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        myFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            Run();
            FlipSprite();
            if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                Jump();
            }

            if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ladder")))
            {
                ClimbLadder();
            }
            else
            {
                myRigidBody.gravityScale = gravityScaleAtStart;
                myanimator.SetBool("Climbing", false);
            }
            if (myBodycollider.IsTouchingLayers(LayerMask.GetMask("Enemy")) || myBodycollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
            {
                Die();
            }
        }
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myanimator.SetBool("Climbing", false);
        }

    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        myanimator.SetBool("Running", Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon);

    }

    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void Die()
    {
        isAlive = false;
        myanimator.SetTrigger("Die");
        GetComponent<Rigidbody2D>().velocity = deathKick;
        StartCoroutine(ReloadLevel());
    }
    private void FlipSprite()
    {

        if (Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon)
        {

            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }

    }
    private void ClimbLadder()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0;
        myanimator.SetBool("Climbing", Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon);
    }

    private IEnumerator ReloadLevel()
    {
        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
