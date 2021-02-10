using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    GridIndex index;
    GameController controller;
    float speed;
    [SerializeField] float turnDuration = 0.2f;
    public bool isMoving;
    GridIndex nextCell;
    Vector2 currentDir;
    Vector2 nextDir;
    Animator animator;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] Color particleColor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (controller.isCaught(index))
        {
            controller.loseLife();
            StartCoroutine(playParticles());
            Destroy(gameObject);
        }
    }

    public void pacmanInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        isMoving = false;
        speed = controller.BASE_SPEED * 2;
        animator = GetComponent<Animator>();
    }

    public void Move(Vector2 direction)
    {
        nextCell = new GridIndex(index.x + (int)direction.x, index.y + (int)direction.y);
        if(!controller.isCellActive(nextCell) && nextDir == Vector2.zero)
            StartCoroutine(moveQueue(direction));
        StartCoroutine(rotate(direction));
    }

    public void Move()
    {
        nextCell = new GridIndex(index.x + (int)currentDir.x, index.y + (int)currentDir.y);
        if (!controller.isCellActive(nextCell))
            StartCoroutine(move(currentDir));
    }

    IEnumerator move(Vector2 dir)
    {
        isMoving = true;
        float time = 0.0f;
        currentDir = dir;
        Vector3 startPos = transform.position;
        Vector3 destPos = transform.position + new Vector3(dir.x, 0.0f, dir.y);

        while(time < speed)
        {
            transform.position = Vector3.Lerp(startPos, destPos, time / speed);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = destPos;
        index = nextCell;

        isMoving = false;
    }

    IEnumerator rotate(Vector2 dir)
    {
        float time = 0.0f;

        while(time < turnDuration)
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

    IEnumerator moveQueue(Vector2 dir)
    {
        nextDir = dir;
        while(isMoving)
            yield return null;
        move(dir);
        nextDir = Vector2.zero;
    }

    IEnumerator playParticles()
    {
        ParticleSystem.MainModule psmain = particleSystem.main;
        psmain.startColor = particleColor;
        ParticleSystem ps = Instantiate(particleSystem, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(ps);
    }
}
