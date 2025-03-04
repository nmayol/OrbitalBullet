using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using UnityEngine.SceneManagement;
//using System;

public class SceneController : MonoBehaviour
{
    private int number_of_rings = 5;

    private bool gameIsFinished = false;
    private bool gameIsStarted = false;

    //////////// Game elements ////////////

    //Press E Billboards
    [SerializeField] private GameObject GoNextLevel;
    [SerializeField] private GameObject TakeJumpPad;

    // Player
    private PlayerLogic player;
    RingIdentifierLogic ringIdentifierLogic;

    // Rings
    private List<Ring> internalRings = new List<Ring>();
    private List<Ring> externalRings = new List<Ring>();
    private float winningRadius = 0.0f;
    private float internalRingRadius = 4.5f;
    private float extrenalRingRadius = 17.0f;


    // Start is called before the first frame update
    void Start()
    {
       init_rings();
       init_player();
       gameIsStarted = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsStarted)
        {
            int currentRing = ringIdentifierLogic.getRingId();
            if (currentRingIsFinished()) {
                if (currentRing != (number_of_rings - 1)) {
                    externalRings[currentRing].turnIndicadorOn();
                    if (playerIsInPositionToGoUp())
                    {
                        GoNextLevel.SetActive(true); // *
                        
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            externalRings[currentRing + 1].triggerPlatformMovementToStart();
                            player.triggerToJumpToTheNextRing();
                            ringIdentifierLogic.setRingId(currentRing + 1);
                            if (currentRing + 1 == 4) FindAnyObjectByType<Boss>().WakeUpBoss();
                            
                        }
                    }
                }
                else
                { 
                    player.winGame();
                    return;
                }
            }
            else GoNextLevel.SetActive(false); // *

            if (playerCanChangeToInternalOrExternalRing()) {
                TakeJumpPad.SetActive(true); // *

                if (Input.GetKeyDown(KeyCode.E))
                {
                    UnityEngine.Vector3 newPlayerCoords;
                    float newRadius;
                    bool isInExternalRing = ringIdentifierLogic.isExternal();
                    if(isInExternalRing) newRadius = internalRingRadius;
                    else newRadius = extrenalRingRadius;

                    player.changeToInternalOrExternalRing(newRadius);
                    ringIdentifierLogic.setExternal(!isInExternalRing);
                    TakeJumpPad.SetActive(false); // *
                }
                
            }
            else {
                TakeJumpPad.SetActive(false); // *
            }

        }
        
    }

    private void init_player()
    {
        player = GameObject.Find("Player").GetComponent<PlayerLogic>();
        ringIdentifierLogic = GameObject.Find("Player").GetComponent<RingIdentifierLogic>();
    }


    private void init_rings() {
       
        //string ringId = "level" + i.ToString();
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level0").GetComponent<Ring>());
        internalRings.Add(GameObject.Find("collisionable/internes/" + "level0").GetComponent<Ring>());
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level1").GetComponent<Ring>());
        internalRings.Add(null);
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level2").GetComponent<Ring>());
        internalRings.Add(GameObject.Find("collisionable/internes/" + "level2 (cos de la nau)").GetComponent<Ring>());
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level3").GetComponent<Ring>());
        internalRings.Add(null);
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level4").GetComponent<Ring>());
        internalRings.Add(GameObject.Find("collisionable/internes/" + "level4").GetComponent<Ring>());

        // externalRings.Add(GameObject.Find("collisionable/externes/" + "level4").GetComponent<Ring>());
        externalRings[0].setPlatformById();
        externalRings[1].setPlatformById();
        externalRings[2].setPlatformById();
        externalRings[3].setPlatformById();
        externalRings[4].setPlatformById();
    }


    private bool playerIsInPositionToGoUp() {
        int currentRing = ringIdentifierLogic.getRingId();
        return externalRings[currentRing + 1].playerIsInPositionToGoUp(GameObject.Find("Player").transform.position, currentRing);
        
    }
    private bool currentRingIsFinished()
    {
        int currentRing = ringIdentifierLogic.getRingId();
        if (currentRing == 4) return externalRings[currentRing].isFinished();
        return externalRings[currentRing].isFinished() && (currentRing % 2 != 0 || internalRings[currentRing].isFinished());
    }

    private bool playerCanChangeToInternalOrExternalRing() {
        return player.canChangeToInternalOrExternalRing();
    }


}
