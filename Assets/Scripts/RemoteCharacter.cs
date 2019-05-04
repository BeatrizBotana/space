using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

// todo: que el remoto este oculto y solo se haga visible al conectarse

public class RemoteCharacter : Character
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        core.Socket.On("remotePlayerStateUpdated", OnRemovePlayerStateUpdated);
    }

    public void OnRemovePlayerStateUpdated(SocketIOEvent e)
    {
        string positionX = "";
        string positionY = "";
        string positionZ = "";
        string rotation = "";

        e.data.GetField(ref positionX, "positionX");
        e.data.GetField(ref positionY, "positionY");
        e.data.GetField(ref positionZ, "positionZ");
        e.data.GetField(ref rotation, "rotation");

        var newPos = new Vector3(float.Parse(positionX), float.Parse(positionY), float.Parse(positionZ));

        transform.position = newPos;
    }

    public override void Respawn(Vector3 position, float orientation)
    {
        transform.position = position;
        // todo: aplicar la rotacion

        base.Respawn(position, orientation);
    }
}
