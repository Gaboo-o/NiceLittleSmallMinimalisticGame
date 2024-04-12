using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using PlayerClass;

public class Viking : MonoBehaviour, CharacterVariables
{
    //Classes
    public MainClass mainClass;
    public VikingAbilities abilities;

    //Movement
    [Header("Movement")]
    [SerializeField]
    float moveSpeed;

    //Dash
    [SerializeField]
    float dashSpeed;
    [SerializeField]
    float dashTimer;

    //Weapon
    [Header("Weapon")]
    [SerializeField]
    GameObject axe;
    [SerializeField]
    GameObject shield;
    [SerializeField]
    GameObject shieldBack;
    [SerializeField]
    Vector3 weaponPosition;

    //Projectile
    [Header("Projectile")]
    [SerializeField]
    GameObject projectile;
    [SerializeField]
    Vector3 projectilePosition;
    [SerializeField]
    int projectileDamage;
    [SerializeField]
    float projectileSpeed;
    [SerializeField]
    float projectileRange;


    CharacterVariables.Variables vars;
    public CharacterVariables.Variables variables()
    {
        return vars;
    }
    CharacterVariables.Variables SetUpVars(CharacterVariables.Variables v)
    {
        v.name = "Viking";

        //Movement
        v.moveSpeed = moveSpeed;
        v.dashSpeed = dashSpeed;
        v.dashTimer = dashTimer;

        //Weapon
        v.weaponPrimary = axe;
        v.weaponSecondary = shield;
        v.weaponTertiary = shieldBack;
        v.weaponPosition = weaponPosition;

        //Projectile
        v.projectile = projectile;
        v.projectilePosition = projectilePosition;
        v.projectileDamage = projectileDamage;
        v.projectileSpeed = projectileSpeed;
        v.projectileRange = projectileRange;

        return v;
    }

    private void Start()
    {
        //*********************************************
        //                Main Class                  *
        //*********************************************

        //Class
        mainClass = new MainClass();

        //Components
        mainClass.name = this.name;
        mainClass.rigidbody = this.GetComponent<Rigidbody2D>();
        mainClass.collider = this.GetComponent<BoxCollider2D>();
        mainClass.animator = this.GetComponentInChildren<Animator>();

        //Movement
        mainClass.moveSpeed = moveSpeed;

        //Dash
        mainClass.dashSpeed = dashSpeed;
        mainClass.dashTimer = dashTimer;

        //*********************************************
        //               Ability Class                *
        //*********************************************

        //Class
        abilities = new VikingAbilities();

        //**********************
        // Weapon / Projectile *
        //**********************

        //GameObjects
        abilities.weaponPrimary = axe;
        abilities.projectile = projectile;
        abilities.weaponSecondary = shield;
        abilities.shieldBack = shieldBack;

        //Position
        abilities.weaponPosition = weaponPosition;
        abilities.projectilePosition = projectilePosition;

        //Values
        abilities.projectileDamage = projectileDamage;
        abilities.projectileSpeed = projectileSpeed;
        abilities.projectileRange = projectileRange;

        //**********************
        //  Special Ability 1  *
        //**********************

        //**********************
        //  Special Ability 2  *
        //**********************

        //**********************
        //    Gettable Vars    *
        //**********************
        vars = new CharacterVariables.Variables();
        vars = SetUpVars(vars);
    }

    private void FixedUpdate()
    {
        mainClass.Move();
        mainClass.Dash();
        abilities.WeaponRotation();

        mainClass.Animations();
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        mainClass.moveDirection = value.ReadValue<Vector2>();
        abilities.moveDirection = value.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            mainClass.PreDash();
        }
    }

    public void OnStop(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            mainClass.Stop();
        }
    }

    public void OnWeaponAttack(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            abilities.WeaponAtack(null, projectile);
        }
    }

    public void OnSpecialAttack1(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            abilities.SpecialAttack1();
        }
    }

    public void OnSpecialAttack2(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            abilities.SpecialAttack2();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CharacterVariables rt = other.GetComponent<CharacterVariables>();

        if (rt != null)
        {
            abilities.cVars = rt;
            abilities.stolenWeapon = rt.variables().weaponPrimary;
            abilities.stolenProjectile = rt.variables().projectile;
        }
    }
}

public class VikingAbilities : Abilities
{
    //*********************************************
    //            Weapon / Projectile             *
    //*********************************************

    //
    public GameObject shieldBack;

    //Values
    public int projectileDamage;
    public float projectileSpeed;
    public float projectileRange;

    //Bools
    bool goBack = false;
    bool hasStolenWeapon = false;

    //*********************************************
    //            Weapon / Projectile             *
    //*********************************************

    public CharacterVariables cVars;
    public GameObject stolenWeapon = null;
    public GameObject stolenProjectile = null;

    public override void WeaponAtack(Quaternion? angle, GameObject pj)
    {
        Axe axeScript = null;

        if (!hasStolenWeapon || stolenWeapon == null || pj != stolenProjectile)
        {
            hasStolenWeapon = false;

            if (!goBack || projectileSpawned == null)
            {
                goBack = true;

                base.WeaponAtack(angle, pj);

                //Spawn projectile & assign vars
                axeScript = projectileSpawned.GetComponent<Axe>();
                axeScript.axeDamage = projectileDamage;
                axeScript.axeSpeed = projectileSpeed;
                axeScript.axeRange = projectileRange;
                axeScript.axeDirection = projectileDirection;

                axeScript.viking = weaponPrimary.transform.parent.gameObject;
                axeScript.axe = weaponPrimary;
                axeScript.shield = weaponSecondary;
                axeScript.shieldBack = shieldBack;

                weaponPrimary.SetActive(false);
            }
            else //return axe
            {
                goBack = false;

                axeScript = projectileSpawned.GetComponent<Axe>();
                axeScript.goBack = true;
                axeScript.hasHit = false;

                axeScript.stolenWeapon = weaponTertiary;
            }
        }
        else //has stolen weapon
        {
            base.WeaponAtack(angle, pj);

            ProjectileVariables stolenP = pj.GetComponent<ProjectileVariables>();
            
            
            /*
            var test = projectileSpawned.GetType("Projectile");
            test.projectileDamage = projectileDamage;
            test.projectileSpeed = projectileSpeed;
            test.projectileRange = projectileRange;
            test.projectileDirection = projectileDirection;
            */
            
            /*
            var bScript = projectileSpawned.GetComponent<FireBall>();
            bScript.DAMAGE = projectileDamage;
            bScript.SPEED = projectileSpeed;
            bScript.RANGE = projectileRange;
            bScript.DIRECTION = projectileDirection;
   
            bScript.character = weaponPrimary.transform.parent.gameObject;
            bScript.FIRE_DPS = 2;*/
        }
    }
    public override void SpecialAttack1()
    {
        if (!weaponPrimary.activeSelf)
        {
            if (weaponSecondary.activeSelf)
            {
                weaponSecondary.SetActive(false);
                shieldBack.SetActive(true);
            }
            else
            {
                weaponSecondary.SetActive(true);
                shieldBack.SetActive(false);
                
                if (weaponTertiary != null)
                {
                    GameObject.Destroy(weaponTertiary);
                    stolenWeapon = null;

                    hasStolenWeapon = false;
                }
            }
        }
    }
    public override void SpecialAttack2()
    {
        if (goBack && stolenWeapon != null) 
        {
            if (weaponTertiary != null)
            {
                WeaponAtack(null, stolenProjectile);
            }
            else
            {
                weaponTertiary = GameObject.Instantiate(stolenWeapon, weaponPosition, Quaternion.identity, weaponPrimary.gameObject.transform.parent);

                weaponSecondary.SetActive(false);
                shieldBack.SetActive(true);

                hasStolenWeapon = true;
            }
        }

        //steal weapon
        //get weapon from enemy
        //get projectile from enemy
        //set axe / shield to false
        //deactivate other abilities
        //
    }
}