using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;
using System;
public class MainMenu : MonoBehaviour
{
    public GameObject uiGameItemPrefab;

    private Core core;
    private Text message;
    private GameObject gamesPanel;
    private InputField playerName;
    private GameObject activeGames;
    private string playerId;

    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.Find("Core").GetComponent<Core>();
        message = GameObject.Find("Canvas/Message").GetComponent<Text>();
        gamesPanel = GameObject.Find("Canvas/Games");
        playerName = gamesPanel.transform.Find("NameInput").GetComponent<InputField>();
        activeGames = gamesPanel.transform.Find("ActiveGames").gameObject;

        playerName.text = "Rabbit";
        gamesPanel.SetActive(false);

        core.Socket.On("connected", OnConnected);
        core.Socket.On("gameCreated", OnGameCreated);
        core.Socket.On("gameJoined", OnGameJoined);
        core.Socket.On("gamesListed", OnGameListed);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnected(SocketIOEvent e)
    {
        e.data.GetField(ref playerId, "id");
        message.enabled = false;
        gamesPanel.SetActive(true);

        // todo: poner un boton refresh para llamar listGames
        core.Socket.Emit("listGames");
    }

    public void OnGameListed(SocketIOEvent e)
    {
        // todo: hacer una lista de verdad y mostrar el nombre en vez del id
        var games = e.data.GetField("games");
        string id = "";
        string name = "";
        int players = 0;

        // Comprobar si hay mas de 2 jugadores
        for (var i = 0; i < games.Count; ++i) {
            var game = games[i];
            game.GetField(ref id, "id");
            game.GetField(ref name, "name");
            game.GetField(ref players, "players");
            if (players <= 2) {
                var go = Instantiate(uiGameItemPrefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(activeGames.transform);
                go.transform.localPosition = Vector3.zero;

                go.transform.Find("Text").GetComponent<Text>().text = id;
                go.transform.GetComponent<Button>().onClick.AddListener(() => JoinGame(id));
            }

        }
    }

    public void OnGameCreated(SocketIOEvent e)
    {
        string gameId = "";
        e.data.GetField(ref gameId, "gameId");

        core.PlayerName = playerName.text;
        core.PlayerId = playerId;
        core.PlayerIsMaster = true;
        core.GameId = gameId;

        SceneManager.LoadScene("Game");
    }

    public void OnGameJoined(SocketIOEvent e)
    {
        string gameId = "";
        e.data.GetField(ref gameId, "gameId");

        core.PlayerName = playerName.text;
        core.PlayerId = playerId;
        core.PlayerIsMaster = false;
        core.GameId = gameId;

        SceneManager.LoadScene("Game");
    }

    public void OnCreateGame()
    {
        var data = new Dictionary<string, string>();
        data["playerId"] = playerId;
        data["playerName"] = playerName.text;

        core.Socket.Emit("createGame", new JSONObject(data));
    }

    public void JoinGame(string gameId)
    {
        var data = new Dictionary<string, string>();
        data["playerId"] = playerId;
        data["gameId"] = gameId;

        core.Socket.Emit("joinGame", new JSONObject(data));
    }
}
