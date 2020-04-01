using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deployEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject collectiblePrefab;
    public int maxCollectibles = 4;
    
    private float respawnTimeEnemies = 0.5f;
    private float respawnTimeCollectibles = 3f;
    private Vector2 screenBounds;
    public float maxX;
    public float maxY;

    private UIScript gameStats;


    // Use this for initialization
    void Start () {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        maxX = screenBounds.x;
        maxY = screenBounds.y;
        gameStats =  GameObject.Find("UserInterface").GetComponent<UIScript>();
        StartCoroutine(enemyWave());
        StartCoroutine(collectiblesGenerator());
    }

    private void addCollectible() {
        
        //set random png
        GameObject collectible = Instantiate(collectiblePrefab) as GameObject;  
        SpriteRenderer spriteRenderer; 
        string spritePath = "Collectible";
        int randomCollNumber = Random.Range(0, maxCollectibles);
        spritePath += randomCollNumber;
        spriteRenderer = collectible.GetComponent<SpriteRenderer>();
        Sprite sp  =  Resources.Load (spritePath, typeof(Sprite)) as Sprite;
        spriteRenderer.sprite = sp;   

        //set size
        float size = collectible.GetComponent<Renderer> ().bounds.size.y;
        Vector3 rescale = collectible.transform.localScale;
        rescale.y = 3 * rescale.y / size;
        rescale.x = 3 * rescale.x / size;
        collectible.transform.localScale = rescale;   

        //set random position
        float paddingConstant = 0.95f;
        collectible.transform.position = 
            new Vector2(Random.Range(-maxX * paddingConstant, maxX * paddingConstant), 
            Random.Range(-maxY * paddingConstant, maxY * paddingConstant));
        
    }   

    private void spawnEnemy(){
        GameObject a = Instantiate(enemyPrefab) as GameObject;        
        a.transform.position = new Vector2(Random.Range(-maxX, maxX), maxY * 2);
    }

    IEnumerator enemyWave(){        
        while(true && !gameStats.gameOver){
            yield return new WaitForSeconds(respawnTimeEnemies);
            spawnEnemy();
        }
    }

    IEnumerator collectiblesGenerator(){
        while(true && !gameStats.gameOver){
            yield return new WaitForSeconds(respawnTimeCollectibles);
            addCollectible();
        }
    }
}
