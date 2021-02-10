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
    Vector2 currentDir;
    Vector2 currentRot;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pacmanInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        isMoving = false;
        speed = controller.BASE_SPEED * 2;
        currentRot = new Vector2(0, 0);
        animator = GetComponent<Animator>();
    }

    public void Move(Vector2 direction)
    {
        GridIndex nextCell = new GridIndex(index.x + (int)direction.x, index.y + (int)direction.y);
        if(!controller.isCellActive(nextCell))
            StartCoroutine(move(direction));
        StartCoroutine(rotate(direction));
    }

    public void Move()
    {
        GridIndex nextCell = new GridIndex(index.x + (int)currentDir.x, index.y + (int)currentDir.y);
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
        index = new GridIndex(index.x + (int)dir.x, index.y + (int)dir.y);

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
}
