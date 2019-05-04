using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class LocalCharacter : Character
{
    private const float SPEED = 6.0f;
    private const float GRAVITY = 20.0f;
    private const float JUMP_SPEED = 8.0f;

    private CharacterController characterController;
    private float yDirection = 0.0f;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (characterController.isGrounded) {
            moveDirection = transform.TransformDirection(moveDirection) * SPEED;
        }
        else
        {
            moveDirection = transform.TransformDirection(new Vector3(moveDirection.x, moveDirection.y, moveDirection.z));
            moveDirection.x *= SPEED;
            moveDirection.z *= SPEED;
        }

        moveDirection.y -= GRAVITY * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void FixedUpdate()
    {
        // todo: hacerlo cada menos tiempo?
        // todo: comprobar que algo ha cambiado en vez de enviar todo siempre
        // send state info
        var data = new Dictionary<string, string>();

        data["playerId"] = core.PlayerId;
        data["positionX"] = transform.position.x.ToString();
        data["positionY"] = transform.position.y.ToString();
        data["positionZ"] = transform.position.z.ToString();
        // data[valores que hacen lanzar una animacion u otra]
        data["rotation"] = transform.rotation.y.ToString();

        core.Socket.Emit("updatePlayerState", new JSONObject(data));
    }

    public void Move(Vector3 direction)
    {
        moveDirection = direction;
        yDirection -= GRAVITY * Time.deltaTime;
        direction.y = yDirection;
        characterController.Move(direction * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public void Jump()
    {
        if (IsGrounded())
            yDirection = JUMP_SPEED;
    }

    public override void Respawn(Vector3 position, float orientation)
    {
        characterController.Move(position - transform.position);
        // todo: aplicar la rotacion

        base.Respawn(position, orientation);
    }
}
