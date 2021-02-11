using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //!!!!Pacman and Ghosts inherit properties from this character class!!!!
    public GridIndex index;
    protected GameController controller;
    protected float speed;
    [SerializeField] protected float turnDuration = 0.2f;
    public bool isMoving;
    protected GridIndex nextCell;
    protected Vector2 currentDir;
    protected Vector2 nextDir;
    protected Animator animator;
    [SerializeField] protected new ParticleSystem particleSystem;
    [SerializeField] protected Color particleColor;

    //Check if character is caught in a wall box and kill the character if true
    private void FixedUpdate()
    {
        if(controller.isCaught(index))
        {
            if (GetComponent<Pacman>() != null) controller.loseLife();
            kill();
        }
    }

    //Move character towards a direction
    public void Move(Vector2 direction)
    {
        nextCell = new GridIndex(index.x + (int)direction.x, index.y + (int)direction.y);
        if (!controller.isCellActive(nextCell))
            StartCoroutine("move", direction);
        if(GetComponent<Pacman>() != null) StartCoroutine("rotate", direction);
    }

    //Auto move to the current direction
    public void Move()
    {
        nextCell = new GridIndex(index.x + (int)currentDir.x, index.y + (int)currentDir.y);
        if (!controller.isCellActive(nextCell))
            StartCoroutine("move", currentDir);
    }

    //Move one cell according to speed
    protected IEnumerator move(Vector2 dir)
    {
        isMoving = true;
        float time = 0.0f;
        currentDir = dir;
        Vector3 startPos = transform.position;
        Vector3 destPos = transform.position + new Vector3(dir.x, 0.0f, dir.y);

        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, destPos, time);
            time += Time.deltaTime * speed;
            yield return null;
        }
        transform.position = destPos;
        index = nextCell;

        isMoving = false;
    }

    //Rotate character using Animator
    protected IEnumerator rotate(Vector2 dir)
    {
        float time = 0.0f;

        while (time < turnDuration)
        {
            float horizontal = Mathf.Lerp(currentDir.x, dir.x, time / turnDuration);
            float vertical = Mathf.Lerp(currentDir.y, dir.y, time / turnDuration);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            time += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        currentDir = dir;
    }

    //Move Queue to allow movement input while already moving
    protected IEnumerator moveQueue(Vector2 dir)
    {
        nextDir = dir;
        while (isMoving)
            yield return null;
        StartCoroutine("move", dir);
        nextDir = Vector2.zero;
    }

    //Play particle system with custom color
    protected IEnumerator playParticles()
    {
        ParticleSystem ps = Instantiate(particleSystem, transform.position, Quaternion.identity);
        ParticleSystem.MainModule psmain = ps.main;
        psmain.startColor = particleColor;
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(ps.gameObject);
    }

    //Kill the character
    public void kill()
    {
        StartCoroutine("playParticles");
        Destroy(gameObject);
    }
}
