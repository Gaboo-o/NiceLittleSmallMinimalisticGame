using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    //GameObjects
    public GameObject spawnedFrom;
    protected GameObject objectHit;

    //Components
    protected Animator projectileAnimator;
    //protected BoxCollider2D projectileCollider;

    //Vectors
    protected Vector2 projectileLastPosition;
    public Vector2 projectileDirection;

    //Values
    [SerializeField]
    protected int projectileDamage;
    [SerializeField]
    protected float projectileSpeed;
    [SerializeField]
    protected float projectileRange;

    //Bools
    protected bool wasDestroyed;
    protected bool hasHit;

    //Functions
    protected virtual void Lifetime()
    {
        if (!wasDestroyed)
        {
            Vector2 currentPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
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
                gameObject.transform.position = newPos;

            }
            else
            {
                gameObject.transform.position = currentPos;
                DestroyProjectile();
            }
        }
    }
    protected virtual void Hit(GameObject other)
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
            //hasHit = false;

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
    protected virtual void DestroyProjectile()
    {
        if (!wasDestroyed)
        {
            wasDestroyed = true;

            projectileLastPosition = gameObject.transform.localPosition;

            GameObject.Destroy(gameObject, 0.2f);
        }
    }


    //*********************************************
    //                InGame                      *
    //*********************************************
    protected void Start()
    {
        projectileAnimator = GetComponent<Animator>();
        //projectileCollider = GetComponent<BoxCollider2D>();
    }

    protected void FixedUpdate()
    {
        Lifetime();
    }
}