using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //TODO: It is okay that there is a Mono/Network Behavior attached to the Player script.
    //Although, this class should do one thing. If there are more things that
    //the Player gameobj should do, then make different classes and call
    //references to those scripts from here.

    float flPlayerMovementSpeed = 0.33f;
    float flPlaneSize = 5f;

    Vector3 vecPlayerPos;

    //Public network variable. What will be used to set the speed. The person wants to move, and the server responds.
    public NetworkVariable<Vector3> netPlayerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> netPlayerColor = new NetworkVariable<Color>(Color.red);

    private GameManager _gameMgr;

    private void Start()
    {
        ApplyPlayerColor();
        netPlayerColor.OnValueChanged += OnPlayerColorChanged;
        //RequestNewPlayerColorServerRpc();
    }

    //Call CalculatePlayerMovement on a regular basis.
    private void Update()
    {
        if (IsOwner)
        {
            if (CalculatePlayerMovement().magnitude > 0)
            {
                RequestPositionForMovementServerRpc(CalculatePlayerMovement());
            }
        }
        else
        {
            transform.position = netPlayerPosition.Value;
        }
    }

    Vector3 CalculatePlayerMovement()
    {
        vecPlayerPos = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        vecPlayerPos *= flPlayerMovementSpeed;

        return vecPlayerPos;
    }

    //Make a server call and request a new position.
    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 movement) //A server name that ends in 'ServerRpc'
    {
        //netPlayerPosition.Value += movement;

        Vector3 vecNewPlayerPosition = netPlayerPosition.Value + movement;

        vecNewPlayerPosition.x = Mathf.Clamp(vecNewPlayerPosition.x, flPlaneSize * -1, flPlaneSize);
        vecNewPlayerPosition.z = Mathf.Clamp(vecNewPlayerPosition.z, flPlaneSize * -1, flPlaneSize);

        netPlayerPosition.Value = vecNewPlayerPosition;
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyPlayerColor(); //This is bad practice. Why would you call a method in a method and that's it? Don't do this.
    }

    public void ApplyPlayerColor()
    {
        GetComponent<MeshRenderer>().material.color = netPlayerColor.Value;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
            _gameMgr.RequestNewPlayerColorServerRpc();
        }
    }

}
