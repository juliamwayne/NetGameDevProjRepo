using Unity.Netcode;
using UnityEngine;

namespace NetworkScripts
{

    public class Player : NetworkBehaviour
    //What is NetworkBehaviour?
    //public class PlayerMovement
    {
        float movementSpeed = 1f;
        Vector3 playerVec;
        GameObject goPlayer;

        public Vector3 CalculatePlayerMovement()
        {
            goPlayer = GameObject.Find("Player");

            //Get current Vector3
            //Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            playerVec = new Vector3(goPlayer.transform.position.x, 0, goPlayer.transform.position.z);

            //Multiply by speed.
            playerVec *= movementSpeed;

            return playerVec;
        }

    }
}

