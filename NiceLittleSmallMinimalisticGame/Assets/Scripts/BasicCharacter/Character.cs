using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    //*********************************************
    //          Movement / Dash / Stop            *
    //*********************************************

    //Components
    protected string charName;
    protected Rigidbody2D charRigidbody;
    protected BoxCollider2D charCollider;
    protected Animator charAnimator;

    //Movement Vars
    [SerializeField]
    protected float moveSpeed;
    protected float directionalSpeed;
    protected Vector2 moveDirection;
    protected bool isMoving = false;

    //Dash Vars
    [SerializeField]
    protected float dashSpeed;
    [SerializeField]
    protected float dashTimer;
    protected float dashTimerCopy;
    protected Vector2 dashDirection;
    protected bool isDashing = false;

    //Stop Vars
    protected bool isStopped = false;

    //Functions
    protected virtual void Move()
    {
        if (!isDashing)
        {
            isMoving = true;

            moveDirection = new Vector2(moveDirection.x, moveDirection.y) * Time.deltaTime;
            directionalSpeed = Mathf.Clamp(moveDirection.magnitude, 0.0f, 1.0f);
            moveDirection.Normalize();

            if (isStopped)
            {
                charRigidbody.velocity = Vector2.zero;
            }
            else
            {
                charRigidbody.velocity = moveDirection * directionalSpeed * moveSpeed;
            }
        }
    }
    protected virtual void PreDash()
    {
        if (!isDashing)
        {
            isDashing = true;
            isMoving = false;

            Vector2 dir = moveDirection;
            dir.Normalize();
            dashDirection = dir;

            dashTimerCopy = dashTimer;
        }
    }
    protected virtual void Dash()
    {
        if (isDashing)
        {
            Vector2 newPos = dashDirection * Time.deltaTime;
            newPos.Normalize();
            charRigidbody.velocity = newPos * dashSpeed;

            dashTimerCopy -= Time.deltaTime;

            if (dashTimerCopy <= 0)
            {
                isDashing = false;
                isMoving = true;
            }
        }
    }
    protected virtual void Stop()
    {
        if (!isDashing)
        {
            if (!isStopped)
            {
                isStopped = true;
                isMoving = false;
            }
            else
            {
                isStopped = false;
                isMoving = true;
            }
        }
    }

    //Functions (Trigger & Input)
    public virtual void OnMove(InputAction.CallbackContext value)
    {
        moveDirection = value.ReadValue<Vector2>();
    }
    public virtual void OnDash(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            PreDash();
        }
    }
    public virtual void OnStop(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Stop();
        }
    }

    //*********************************************
    //                 Animations                 *
    //*********************************************

    //Functions
    protected virtual void Animations()
    {
        if (isMoving)
        {
            if (moveDirection != Vector2.zero)
            {
                charAnimator.SetFloat("Horizontal", moveDirection.x);
                charAnimator.SetFloat("Vertical", moveDirection.y);
                charAnimator.SetFloat("Speed", moveSpeed);
            }
            else if (moveDirection == Vector2.zero)
            {
                charAnimator.SetFloat("Speed", 0);
            }
        }
        else
        {
            charAnimator.SetFloat("Speed", 0);
        }
    }

    //*********************************************
    //                  Weapon                    *
    //*********************************************

    //GameObject
    protected GameObject weaponCurrent;
    [SerializeField]
    protected GameObject weaponPrimary;
    [SerializeField]
    protected GameObject weaponSecondary;
    [SerializeField]
    protected GameObject weaponTertiary = null;

    //Vectors
    [SerializeField]
    protected Vector2 weaponPosition;

    //*********************************************
    //                Projectile                  *
    //*********************************************

    //GameObjects
    [SerializeField]
    protected GameObject projectile;
    protected GameObject projectileSpawned;

    //Vectors
    [SerializeField]
    protected Vector3 projectilePosition;
    protected Vector2 projectileDirection;

    //Functions
    protected void WeaponRotation()
    {
        if (weaponPrimary != null && weaponPrimary.activeSelf)
        {
            weaponCurrent = weaponPrimary;
        }
        else if (weaponSecondary != null && weaponSecondary.activeSelf)
        {
            weaponCurrent = weaponSecondary;
        }
        else if (weaponTertiary != null && weaponTertiary.activeSelf)
        {
            weaponCurrent = weaponTertiary;
        }
        else
        {
            weaponCurrent = null;
        }

        if (moveDirection != Vector2.zero && weaponCurrent != null)
        {
            weaponCurrent.transform.localPosition = moveDirection * weaponPosition;
            weaponCurrent.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(moveDirection.y, moveDirection.x) * 180 / Mathf.PI);

            if (moveDirection.y <= 0 && moveDirection.x <= 0 ||
                moveDirection.y > 0 && moveDirection.x < 0)
            {
                weaponCurrent.GetComponent<SpriteRenderer>().flipY = true;
            }
            else
            {
                weaponCurrent.GetComponent<SpriteRenderer>().flipY = false;
            }
        }
    }
    protected virtual void WeaponAtack(Quaternion? angle, GameObject pj)
    {
        if (angle == null)
        {
            angle = Quaternion.identity;
        }

        projectileDirection = weaponCurrent.transform.localPosition;
        projectileDirection.Normalize();

        projectileSpawned = GameObject.Instantiate(pj, projectilePosition + weaponCurrent.transform.position, (Quaternion)angle);
        projectileSpawned.transform.Rotate(0, 0, Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg);

        var projectileScript = projectileSpawned.GetComponent<Projectile2>();
        projectileScript.projectileDirection = projectileDirection;
        projectileScript.spawnedFrom = gameObject;
    }

    //Functions (Trigger & Input)
    public virtual void OnWeaponAttack(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            WeaponAtack(null, projectile);
        }
    }

    //*********************************************
    //                Abilities                   *
    //*********************************************

    //Functions
    protected virtual void AbilityOne()
    {

    }
    protected virtual void AbilityTwo()
    {

    }
    protected virtual void PassiveAbility()
    {

    }

    //Functions (Trigger & Input)
    public virtual void OnAbilityOne(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            AbilityOne();
        }
    }
    public virtual void OnAbilityTwo(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            AbilityTwo();
        }
    }

    //*********************************************
    //                InGame                      *
    //*********************************************
    protected void Start()
    {
        //*********************************************
        //          Movement / Dash / Stop            *
        //*********************************************

        //Components
        charName = this.name;
        charRigidbody = this.GetComponent<Rigidbody2D>();
        charCollider = this.GetComponent<BoxCollider2D>();
        charAnimator = this.GetComponentInChildren<Animator>();
    }

    protected void FixedUpdate()
    {
        Move();
        Dash();
        WeaponRotation();

        PassiveAbility();

        Animations();
    }
}
