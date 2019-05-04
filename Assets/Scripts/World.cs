using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class World : MonoBehaviour
{
    private const float MAX_DISTANCE = 1.0f;
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Character> players = new List<Character>();
    private Core core;
    private Character localCharacter;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform point in GameObject.Find("SpawnPoints").transform)
        {
            spawnPoints.Add(point);
        }

        core = GameObject.Find("Core").GetComponent<Core>();
        core.Socket.On("remoteGetStartGameInfo", OnRemoteGetStartGameInfo);
        core.Socket.On("playerStartGame", OnPlayerStartGame);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LocalPlayerStartGame()
    {
        if (core.PlayerIsMaster)
        {
            var player = GameObject.Find("Local").GetComponent<LocalCharacter>();
            var spawnPoint = GetSpawnPoint();

            players.Add(player);
            player.Respawn(spawnPoint.position, spawnPoint.rotation.y);
            localCharacter = player;
        }
        else
        {
            var data = new Dictionary<string, string>();

            data["playerId"] = core.PlayerId;
            data["gameId"] = core.GameId;

            core.Socket.Emit("remoteGetStartGameInfo", new JSONObject(data));
        }
    }

    public void OnPlayerStartGame(SocketIOEvent e)
    {
        string respawnPosX = "";
        string respawnPosY = "";
        string respawnPosZ = "";
        string respawnRot = "";

        e.data.GetField(ref respawnPosX, "respawnPositionX");
        e.data.GetField(ref respawnPosY, "respawnPositionY");
        e.data.GetField(ref respawnPosZ, "respawnPositionZ");
        e.data.GetField(ref respawnRot, "respawnRotation");

        if (core.PlayerIsMaster)
        {
            var remote = GameObject.Find("Remote").GetComponent<RemoteCharacter>();
            players.Add(remote);
            remote.Respawn(new Vector3(float.Parse(respawnPosX), float.Parse(respawnPosY), float.Parse(respawnPosZ)), float.Parse(respawnRot));
        }
        else
        {
            string masterPosX = "";
            string masterPosY = "";
            string masterPosZ = "";
            string masterRot = "";

            e.data.GetField(ref masterPosX, "masterPositionX");
            e.data.GetField(ref masterPosY, "masterPositionY");
            e.data.GetField(ref masterPosZ, "masterPositionZ");
            e.data.GetField(ref masterRot, "masterRotation");

            var player = GameObject.Find("Local").GetComponent<LocalCharacter>();
            players.Add(player);
            player.Respawn(new Vector3(float.Parse(respawnPosX), float.Parse(respawnPosY), float.Parse(respawnPosZ)), float.Parse(respawnRot));
            localCharacter = player;

            var remote = GameObject.Find("Remote").GetComponent<RemoteCharacter>();
            players.Add(remote);
            remote.Respawn(new Vector3(float.Parse(masterPosX), float.Parse(masterPosY), float.Parse(masterPosZ)), float.Parse(masterRot));
        }
    }

    public void OnRemoteGetStartGameInfo(SocketIOEvent e)
    {
        var spawnPoint = GetSpawnPoint();

        string playerId = "";
        e.data.GetField(ref playerId, "playerId");

        var data = new Dictionary<string, string>();

        // todo: enviar el estado de todos los objetos del juego

        data["playerId"] = playerId;
        data["gameId"] = core.GameId;
        data["respawnPositionX"] = spawnPoint.position.x.ToString();
        data["respawnPositionY"] = spawnPoint.position.y.ToString();
        data["respawnPositionZ"] = spawnPoint.position.z.ToString();
        data["respawnRotation"] = spawnPoint.rotation.y.ToString();
        // todo: si se soportan mas player pasar aqui la posicion de todos
        data["masterPositionX"] = localCharacter.transform.position.x.ToString();
        data["masterPositionY"] = localCharacter.transform.position.x.ToString();
        data["masterPositionZ"] = localCharacter.transform.position.x.ToString();
        data["masterRotation"] = localCharacter.transform.rotation.y.ToString();

        core.Socket.Emit("playerStartGame", new JSONObject(data));
    }

    public Transform GetSpawnPoint()
    {
        foreach(var point in spawnPoints)
        {
            foreach(var player in players)
            {
                if (player.gameObject.activeSelf && Vector3.Distance(player.transform.position, point.position) > MAX_DISTANCE)
                {
                    return point;
                }
            }
        }

        return spawnPoints[0];
    }
}
