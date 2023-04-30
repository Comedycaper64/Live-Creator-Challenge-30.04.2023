using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    private FloorTile[] floorTiles;
    
    private TextMeshProUGUI colourText;

    private void Awake() 
    {
        Instance = this;    
        GameObject playArea = GameObject.FindGameObjectWithTag("Floor");
        floorTiles = playArea.GetComponentsInChildren<FloorTile>();
        colourText = GameObject.FindGameObjectWithTag("ColourText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() 
    {
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }    
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        colourText.text = "Current Colour: ???";
        ResetFloorTiles();
        yield return new WaitForSeconds(2f);
        FloorTile.FloorColour currentColour = (FloorTile.FloorColour) Random.Range(0, 3);
        colourText.text = "Current Colour: " + currentColour.ToString();
        yield return new WaitForSeconds(2f);
        foreach (FloorTile tile in floorTiles)
        {
            tile.LowerTile(currentColour);
        }
        yield return new WaitForSeconds(2f);

        StartCoroutine(GameLoop());
    }

    private void ResetFloorTiles()
    {
        foreach(FloorTile tile in floorTiles)
        {
            tile.transform.position = new Vector3(tile.transform.position.x, -0.5f, tile.transform.position.z);
            tile.StopLowering();
        }
    }

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }
}
