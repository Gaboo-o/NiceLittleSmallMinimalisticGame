using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using PlayerClass;

public class Axe : MonoBehaviour, ProjectileVariables
{
    //Class
    AxeProperties axeClass;

    //GameObjects
    public GameObject viking;
    public GameObject axe; //not projectile
    public GameObject shield;
    public GameObject shieldBack;
    public GameObject stolenWeapon;

    //Components
    Animator axeAnimator;
    BoxCollider2D axeCollider;

    //Values
    public int axeDamage;
    public float axeSpeed;
    public float axeRange;
    public Vector2 axeDirection;

    //Bools
    public bool goBack { get { return axeClass.goBack; } set { axeClass.goBack = value; } }
    public bool hasHit { get { return axeClass.hasHit; } set { axeClass.hasHit = value; } }

    ProjectileVariables.Variables vars;
    public ProjectileVariables.Variables variables()
    {
        return vars;
    }
    public ProjectileVariables.Variables SetUpVars(ProjectileVariables.Variables v)
    {
        v.name = "Axe";

        //Values
        v.damage = axeDamage;
        v.speed = axeSpeed;
        v.range = axeRange;
        v.direction = axeDirection;
        return v;
    }

    private void Start()
    {
        //**********************
        //    Gettable Vars    *
        //**********************
        vars = new ProjectileVariables.Variables();
        vars = SetUpVars(vars);

        //Class
        axeClass = new AxeProperties();

        //GameObject
        axeClass.projectileSpawned = this.gameObject;
        axeClass.spawnedFrom = viking;

        //Components
        axeAnimator = GetComponent<Animator>();
        axeClass.projectileAnimator = axeAnimator;
        axeCollider = GetComponent<BoxCollider2D>();
        axeClass.projectileCollider = axeCollider;
        axeCollider.enabled = false;

        //Values
        axeClass.projectileDamage = axeDamage;
        axeClass.projectileSpeed = axeSpeed;
        axeClass.projectileRange = axeRange;
        axeClass.projectileDirection = axeDirection;
    }

    private void Update()
    {
        axeClass.Lifetime();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(axeClass.spawnedFrom != null)
        {
            if (other.name == axeClass.spawnedFrom.name)
            {
                if (shield.activeSelf)
                {
                    shield.SetActive(false);
                    shieldBack.SetActive(true);
                }

                if (stolenWeapon)
                {
                    Destroy(stolenWeapon);
                }
                axe.SetActive(true);

                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.Log("null");
        }
    }
}

public class AxeProperties : Projectile
{
    //Bools
    bool colliderActive = false;
    public bool goBack = false;

    public override void Lifetime()
    {
        if (!goBack)
        {
            base.Lifetime();
        } 
        else
        {
            GoBack();
        }
    }

    public override void Hit(GameObject other)
    {
        base.Hit(other);
    }

    public override void DestroyProjectile()
    {
        if (!colliderActive)
        {
            wasDestroyed = true;

            colliderActive = true;
            projectileCollider.enabled = true;

            projectileAnimator = projectileSpawned.GetComponent<Animator>();
            projectileAnimator.speed = 0;
        }
        if (goBack)
        {
            return;
        }
    }

    void GoBack()
    {
        if (!colliderActive)
        {
            colliderActive = true;
            projectileCollider.enabled = true;
        }

        projectileAnimator.speed = 1;

        var goBackSpeed = projectileSpeed * Time.deltaTime;
        var goBackPos = Vector2.MoveTowards(projectileSpawned.transform.position, spawnedFrom.transform.position, goBackSpeed);
        projectileSpawned.transform.position = goBackPos;

        Debug.DrawLine(projectileSpawned.transform.position, goBackPos, Color.yellow);

        if (!hasHit)
        {
            RaycastHit2D hit = Physics2D.Linecast(projectileSpawned.transform.position, goBackPos);

            if (hit && hit.collider != projectileCollider)
            {
                hasHit = true;

                objectHit = hit.collider.gameObject;
                Hit(objectHit);
            }
        }
    }
}