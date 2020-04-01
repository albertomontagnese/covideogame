using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDefaultScript : MonoBehaviour
{
    public float speed = 10.0f;
    private Rigidbody2D rb;
    private Vector2 screenBounds;
    private SpriteRenderer spriteRenderer; 
    
    private string spritePath = "./Images/Creatures/Alien1.png";




    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        // Sprite sp  = Resources.Load<Sprite>(spritePath);
        // spriteRenderer.sprite = sp;


    }

    // Update is called once per frame
    void Update () {
        if(transform.position.x < screenBounds.x * -2){
            Destroy(this.gameObject);
        }
    }
}