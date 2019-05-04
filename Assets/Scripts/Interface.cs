using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Interface : MonoBehaviour
{
    public LocalCharacter localCharacter;
    private World world;
    private bool firstUpdate = true;

    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstUpdate)
        {
            world.LocalPlayerStartGame();
            firstUpdate = false;
        }

        if (!localCharacter.IsActive())
            return;

        // ...
        var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKeyDown("space"))
        {
            localCharacter.Jump();
        }

        localCharacter.Move(moveDirection);
    }
}
