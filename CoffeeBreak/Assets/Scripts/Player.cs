﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour{

    private int coins;
    public float moveSpeed;

    public float throwSpeed;
    private Animator playerAnimator;

    private bool hasCup; 

    private bool playerMoving;
    private Vector2 lastMove;

    private Rigidbody2D rb;

   
    private void Start() {
        coins = GameManager.instance.initialPlayerCoins;
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coins = GameManager.instance.initialPlayerCoins;
        hasCup = false;
    }

    private void Update() {
        PlayerMovement();
        Debug.Log(hasCup);
    }

    

    private void PlayerMovement(){
        playerMoving = false;
        if(Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f){
            playerMoving = true;
            lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            if (Input.GetButton("SlowMovement")){
                playerAnimator.speed = 0.5f;
                transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * moveSpeed / 2f * Time.deltaTime);
            }
            else{
                playerAnimator.speed = 1f;
                transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * moveSpeed * Time.deltaTime);
            }
        }

        if(Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f){
            playerMoving = true;
            lastMove = new Vector2(0f,Input.GetAxisRaw("Vertical"));
            if (Input.GetButton("SlowMovement")){
                playerAnimator.speed = 0.5f;
                transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f) * moveSpeed/2f * Time.deltaTime);
            }
            else{
                playerAnimator.speed = 1f;
                transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f) * moveSpeed * Time.deltaTime);
            }
        }

        playerAnimator.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
        playerAnimator.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        playerAnimator.SetBool("PlayerMoving", playerMoving);
        playerAnimator.SetFloat("LastMoveX", lastMove.x);
        playerAnimator.SetFloat("LastMoveY", lastMove.y);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Coin")){
            Destroy(other.gameObject);
            coins++;
        }

        else if(other.gameObject.CompareTag("Cup")){
            Destroy(other.gameObject);
            hasCup = true;
        }
    }

}
