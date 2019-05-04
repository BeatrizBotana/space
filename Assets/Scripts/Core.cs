using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Core : MonoBehaviour
{
    private SocketIOComponent socket;

    public string PlayerId {get; set;}
    public string PlayerName {get; set;}
    public bool PlayerIsMaster {get; set;}
    public string GameId {get; set;}

    // Start is called before the first frame update
    void Awake()
    {
        Object.DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        socket = transform.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public SocketIOComponent Socket
    {
        get { return socket; }
    }
}
