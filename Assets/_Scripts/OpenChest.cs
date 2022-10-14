using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using _Scripts;
using _Scripts._Items;
using _Scripts._Player;
using UnityEditor;
using UnityEngine;

public class OpenChest : Interactable {
    private Animator animator;
    private OpenChest openChest;

    public GameObject itemSpawner;
    public WeaponItem itemInChest;

    [SerializeField] private float playerRotationSpeed = 300;
    [SerializeField] private Transform playerStandingPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        openChest = GetComponent<OpenChest>();
    }

    public override void Interact(PlayerManager playerManager)
    {
        Vector3 rotationDirection = transform.position - playerManager.transform.position;
        rotationDirection.y = 0;
        rotationDirection.Normalize();

        Quaternion tr = Quaternion.LookRotation(rotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, playerRotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

        playerManager.OpenChestInteraction(playerStandingPosition);
        animator.Play("Chest Open");
        StartCoroutine(SpawnItemChest()); 
        
        WeaponPickUp weaponPickUp = itemSpawner.GetComponent<WeaponPickUp>();

        if ( weaponPickUp != null )
        {
            weaponPickUp.weapon = itemInChest;
        }
        // Rotate Player towards to the Chest 
        //lock his transform to a certain point infrot of it 
        // open the chest lid and animate the player 
        // spawn an item inside
    }

    private IEnumerator SpawnItemChest()
    {
        yield return new WaitForSeconds(1);
        Instantiate(itemSpawner, transform);
        Destroy(openChest);
    }
}