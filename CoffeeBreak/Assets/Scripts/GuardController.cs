﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Player;


public class GuardController : AIController {

    public Light flashlight;

    public AudioClip runningSound;
    public float rotationSpeed;
    public float awerenessRadius;

    public GameObject alertIcon;

    private GameObject targetCoin = null;

    private Renderer alertRenderer;
    private float stopThreshold = 0.20f;

    private bool spottedPlayer = false;

    protected override void Start () {
        base.Start ();
        alertRenderer = alertIcon.GetComponent<Renderer>();
        alertRenderer.enabled = false;
    }

    protected override void Update () {
        base.Update ();
        Vector3 direction = Vector3.Normalize (agent.velocity);
        flashlight.transform.forward = Vector3.RotateTowards (flashlight.transform.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        HandleDistraction ();
    }

    protected void FixedUpdate () {
        RaycastHit2D[] hit = Physics2D.RaycastAll ((Vector2) gameObject.transform.position + (Vector2) flashlight.transform.forward.normalized, ((Vector2) flashlight.transform.forward.normalized), 4.5f);
        //Debug.DrawLine ((Vector2) gameObject.transform.position + (Vector2) flashlight.transform.forward.normalized, (Vector2) gameObject.transform.position + (Vector2) flashlight.transform.forward.normalized * 4.5f);
        Debug.DrawRay(gameObject.transform.position +flashlight.transform.forward.normalized ,((Vector2) flashlight.transform.forward.normalized*4.5f));
        if (hit.Length > 1 ) {
            if (hit[1].collider.CompareTag ("Player") && !spottedPlayer) {
                spottedPlayer = true;
                Player player = (Player) hit[1].collider.gameObject.GetComponent ("Player");
                StartCoroutine(HandleSeen(player));
            }
        }
    }

    private void HandleDistraction () {
        Debug.DrawLine (transform.position, transform.position + new Vector3 (awerenessRadius, awerenessRadius, 0)); //comment for test
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll (transform.position, awerenessRadius);
        foreach (Collider2D col in hitColliders) {
            if (col.tag == "ThrownCoin" && targetCoin == null) {
                targetCoin = col.gameObject;
            } else if (col.tag == "Player") {
                Player playerController = (Player) col.gameObject.GetComponent ("Player");
                if (!playerController.IsStealth () && !spottedPlayer) {
                    spottedPlayer = true;
                    StartCoroutine(HandleSeen(playerController));
                }
            }
        }

        if (targetCoin != null) {
            Rigidbody2D rbCoin = targetCoin.GetComponent<Rigidbody2D> ();

            //stopped coin
            if (rbCoin.velocity.x <= stopThreshold && rbCoin.velocity.y <= stopThreshold) {
                target = rbCoin.position;
            }

            //on coin
            if (agent.remainingDistance < 1f) {
                Destroy (targetCoin);
                targetCoin = null;
            }
        }
    }

    private IEnumerator HandleSeen(Player player){
        StartCoroutine(DoBlinks(1.5f, 0.1f));
        target = player.gameObject.transform.position;
        yield return new WaitForSeconds(0.5f);
        GotoNextPoint();
        player.looseLife();
        player.gameObject.transform.position = player.respawnPoint.position;
        spottedPlayer = false;
    }

    private IEnumerator DoBlinks(float duration, float blinkTime) {
        while (duration >= 0f) {
            duration -= (Time.deltaTime + blinkTime);
            alertRenderer.enabled = !alertRenderer.enabled;
            yield return new WaitForSeconds(blinkTime);
        }
        alertRenderer.enabled = false;
        spottedPlayer = false;
    }
}