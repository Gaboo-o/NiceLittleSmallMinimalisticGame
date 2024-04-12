using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerClass
{
    public interface CharacterVariables
    {
        public struct Variables
        {
            //*********************************************
            //                  Player                    *
            //*********************************************

            public string name;
            public float moveSpeed;
            public float dashSpeed;
            public float dashTimer;

            //*********************************************
            //                  Weapon                    *
            //*********************************************

            public GameObject weaponPrimary;
            public GameObject weaponSecondary;
            public GameObject weaponTertiary;
            public Vector3 weaponPosition;

            //*********************************************
            //                Projectile                  *
            //*********************************************

            public GameObject projectile;
            public Vector3 projectilePosition;
            public int projectileDamage;
            public float projectileSpeed;
            public float projectileRange;
        }
        public Variables variables();
    }

    public interface ProjectileVariables
    {
        public struct Variables
        {
            public string name;

            //GameObjects
            public GameObject character;
            public GameObject[] gameObjects;

            //Components
            public Animator animator;
            public BoxCollider2D collider;

            //Values
            public int damage;
            public float speed;
            public float range;
            public Vector2 direction;
        }
        public Variables variables();
    }

    public class MainClass
    {
        //*********************************************
        //             Main Components                *
        //*********************************************

        public string name;
        public Rigidbody2D rigidbody;
        public BoxCollider2D collider;
        public Animator animator;

        //*********************************************
        //          Movement / Dash / Stop            *
        //*********************************************

        //Movement Vars
        public float moveSpeed;
        protected float directionalSpeed;
        public Vector2 moveDirection;
        protected bool isMoving = false;

        //Dash Vars
        public float dashSpeed;
        public float dashTimer;
        protected float dashTimerCopy;
        protected Vector2 dashDirection;
        protected bool isDashing = false;

        //Stopped Vars
        protected bool isStopped = false;

        //Functions
        public void Move()
        {
            if (!isDashing)
            {
                isMoving = true;

                moveDirection = new Vector2(moveDirection.x, moveDirection.y) * Time.deltaTime;
                directionalSpeed = Mathf.Clamp(moveDirection.magnitude, 0.0f, 1.0f);
                moveDirection.Normalize();

                if (isStopped)
                {
                    rigidbody.velocity = Vector2.zero;
                }
                else
                {
                    rigidbody.velocity = moveDirection * directionalSpeed * moveSpeed;
                }
            }
        }
        public void PreDash()
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
        public void Dash()
        {
            if (isDashing)
            {
                Vector2 newPos = dashDirection * Time.deltaTime;
                newPos.Normalize();
                rigidbody.velocity = newPos * dashSpeed;

                dashTimerCopy -= Time.deltaTime;

                if (dashTimerCopy <= 0)
                {
                    isDashing = false;
                    isMoving = true;
                }
            }
        }
        public void Stop()
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

        //*********************************************
        //                 Animations                 *
        //*********************************************

        //Functions
        public void Animations()
        {
            if (isMoving)
            {
                if (moveDirection != Vector2.zero)
                {
                    animator.SetFloat("Horizontal", moveDirection.x);
                    animator.SetFloat("Vertical", moveDirection.y);
                    animator.SetFloat("Speed", moveSpeed);
                }
                else if (moveDirection == Vector2.zero)
                {
                    animator.SetFloat("Speed", 0);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }

    public class HealthPoints : MonoBehaviour
    {
        //Components
        public HealthBar HPbar;

        //Values
        private int maxHealth;
        private int currentHealth;
        public int health { 
            get { return currentHealth; } 
            set { maxHealth = value; currentHealth = maxHealth; HPbar.SetMaxHealth(maxHealth); } }
        public bool isInvencible = false;

        //*********************************************
        //                Functions                   *
        //*********************************************
        public void TakeDamage(int dmg)
        {
            if (!isInvencible)
            {
                currentHealth -= dmg;

                HPbar.SetHealth(currentHealth);
            }
            Dying();
        }
        public void Healing(int heal)
        {
            currentHealth += heal;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            HPbar.SetHealth(currentHealth);
        }
        void Dying()
        {
            if (currentHealth <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    public class Weapon
    {
        //Movement (From MainClass)
        public Vector2 moveDirection;

        //*********************************************
        //                  Weapon                    *
        //*********************************************

        //GameObject
        public GameObject weaponCurrent;
        public GameObject weaponPrimary;
        public GameObject weaponSecondary;
        public GameObject weaponTertiary = null;

        //Vectors
        public Vector2 weaponPosition;

        //*********************************************
        //                Projectile                  *
        //*********************************************

        //GameObjects
        public GameObject projectile;
        public GameObject projectileSpawned;

        //Vectors
        public Vector3 projectilePosition;
        public Vector2 projectileDirection;

        //Functions
        public void WeaponRotation()
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
        public virtual void WeaponAtack(Quaternion? angle, GameObject pj)
        {
            if (angle == null)
            {
                angle = Quaternion.identity;
            }

            projectileDirection = weaponCurrent.transform.localPosition;
            projectileDirection.Normalize();

            projectileSpawned = GameObject.Instantiate(pj, projectilePosition + weaponCurrent.transform.position, (Quaternion)angle);
            projectileSpawned.transform.Rotate(0, 0, Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg);
        }
    }

    public abstract class Projectile
    {
        //GameObjects
        public GameObject spawnedFrom;
        public GameObject projectileSpawned;
        public GameObject objectHit;

        //Components
        public Animator projectileAnimator;
        public BoxCollider2D projectileCollider;

        //Vectors
        public Vector2 projectileLastPosition;
        public Vector2 projectileDirection;

        //Values
        public int projectileDamage;
        public float projectileSpeed;
        public float projectileRange;

        //Bools
        protected bool wasDestroyed;
        public bool hasHit;

        //Functions
        public virtual void Lifetime()
        {
            if (!wasDestroyed)
            {
                Vector2 currentPos = new Vector2(projectileSpawned.transform.position.x, projectileSpawned.transform.position.y);
                Vector2 newPos = currentPos + (projectileDirection * projectileSpeed) * Time.deltaTime;

                Debug.DrawLine(currentPos, newPos, Color.red);

                if (!hasHit)
                {
                    RaycastHit2D hit = Physics2D.Linecast(currentPos, newPos);

                    if (hit)
                    {
                        hasHit = true;

                        objectHit = hit.collider.gameObject;
                        Hit(objectHit);
                    }
                }

                if (TraveledDistance() && !hasHit)
                {
                    projectileSpawned.transform.position = newPos;

                }
                else
                {
                    projectileSpawned.transform.position = currentPos;
                    DestroyProjectile();
                }
            }
        }
        public virtual void Hit(GameObject other)
        {
            if (other.GetComponent<Health>() != null)
            {
                if (other.name != spawnedFrom.name)
                {
                    other.GetComponent<Health>().OnTakeDamage(projectileDamage);
                    Debug.Log(other.name + " took " + projectileDamage + " damage!");

                    DestroyProjectile();
                }
                else
                {
                    hasHit = false;
                }
            }
            else
            {
                Debug.Log(other.name + " has no Health component");
                hasHit = false;

                DestroyProjectile();
            }
        }
        private bool TraveledDistance()
        {
            projectileRange -= Time.deltaTime;
            if (projectileRange <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public virtual void DestroyProjectile()
        {
            if (!wasDestroyed)
            {
                wasDestroyed = true;

                projectileLastPosition = projectileSpawned.transform.localPosition;

                GameObject.Destroy(projectileSpawned, 0.2f);
            }
        }
    }

    public class Abilities : Weapon
    {
        public virtual void WeaponAttack()
        {

        }
        public virtual void SpecialAttack1()
        {

        }
        public virtual void SpecialAttack2()
        {

        }
    }
}