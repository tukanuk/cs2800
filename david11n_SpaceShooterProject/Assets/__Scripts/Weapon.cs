﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up. 
/// Items marked [NI] below are Not Implemented in the IGDPD book. 
/// </summary>
public enum WeaponType
{
    none,    // The default / no weapon 
    blaster, // A simple blaster
    spread,  // Two shots simultaneously 
    phaser,  // [NI] Shots that move in waves 
    missile, // [NI] Homing missiles
    laser,   // [NI]Damage over time
    shield   // Raise shieldLevel
}

/// <summary> 
/// The WeaponDefinition class allows you to set the properties 
///   of a specific weapon in the Inspector. The Main class has 
///   an array of WeaponDefinitions that makes this possible. 
/// </summary> ”
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePreFab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
}



public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    // blaster sound
    AudioSource blasterAS;


    // Start is called before the first frame update
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default type
        SetType(_type);

        // Dynamicall create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Player>() != null)
        {
            rootGO.GetComponent<Player>().fireDelegate += Fire;
        }

        // setup the blaster sound
        GameObject goBs = GameObject.Find("blasterAS");
        blasterAS = goBs.GetComponent<AudioSource>();

         if (blasterAS.clip == null)
        {
            blasterAS.clip = Resources.Load("Audio/SFX/weapon_player", typeof(AudioClip) ) as AudioClip;
        }

    }

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;

        // if hasn't had enough time betwen shots return
        if (Time.time - lastShotTime < def.delayBetweenShots) return;

        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }

        // p[ay the blaster sound
        blasterAS.Play(); 
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePreFab); 
        if (transform.parent.gameObject.tag == "_Player")
        { 
            go.tag = "ProjectilePlayer";
            go.layer = LayerMask.NameToLayer("ProjectilePlayer");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position; 
        go.transform.SetParent(PROJECTILE_ANCHOR, true); 
        Projectile p = go.GetComponent<Projectile>(); 
        p.type = type;
        lastShotTime = Time.time; 
        return (p);
    }
}
