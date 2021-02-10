using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected GridIndex index;
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

    void FixedUpdate()
    {
        if (controller.isCaught(index))
        {
            Pacman pacman;
            if(TryGetComponent(out pacman))
                controller.loseLife();
            StartCoroutine(playParticles());
            Destroy(gameObject);
        }
    }

    public void Move(Vector2 direction)
    {
        nextCell = new GridIndex(index.x + (int)direction.x, index.y + (int)direction.y);
        if (!controller.isCellActive(nextCell) && nextDir == Vector2.zero)
            StartCoroutine(moveQueue(direction));
        StartCoroutine(rotate(direction));
    }

    public void Move()
    {
        nextCell = new GridIndex(index.x + (int)currentDir.x, index.y + (int)currentDir.y);
        if (!controller.isCellActive(nextCell))
            StartCoroutine(move(currentDir));
    }

    protected IEnumerator move(Vector2 dir)
    {
        isMoving = true;
        float time = 0.0f;
        currentDir = dir;
        Vector3 startPos = transform.position;
        Vector3 destPos = transform.position + new Vector3(dir.x, 0.0f, dir.y);

        while (time < speed)
        {
            transform.position = Vector3.Lerp(startPos, destPos, time / speed);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = destPos;
        index = nextCell;

        isMoving = false;
    }

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

    protected IEnumerator moveQueue(Vector2 dir)
    {
        nextDir = dir;
        while (isMoving)
            yield return null;
        move(dir);
        nextDir = Vector2.zero;
    }

    protected IEnumerator playParticles()
    {
        ParticleSystem ps = Instantiate(particleSystem, transform.position, Quaternion.identity);
        ParticleSystem.MainModule psmain = ps.main;
        psmain.startColor = particleColor;
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(ps.gameObject);
    }
}
