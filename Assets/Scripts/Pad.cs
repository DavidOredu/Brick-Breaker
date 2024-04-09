using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float boundX = 50f;

    private float direction;

    private Vector3 mousePos;

    private InputType inputType = InputType.Pointer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxis("Horizontal");

    //    RestrictToBound();
    }
    private void FixedUpdate()
    {
        if(GameManager.instance.hasGameStarted)
            Move();
    }
    private void Move()
    {
        switch (inputType)
        {
            case InputType.Keyboard:
                rb.velocity = new Vector2(moveSpeed * direction * Time.deltaTime, 0f);
                break;
            case InputType.Pointer:
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(mousePos.x, transform.position.y, transform.position.z);
                break;
        }
    }
    private void RestrictToBound()
    {
        var pos = transform.position;

        if(pos.x > boundX)
        {
            pos.x = boundX;
        }
        else if(pos.x < -boundX)
        {
            pos.x = -boundX;
        }
        transform.position = pos;
    }
private enum InputType
{
    Keyboard,
    Pointer,
}
}
