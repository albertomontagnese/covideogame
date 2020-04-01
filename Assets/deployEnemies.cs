using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deployEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject collectiblePrefab;
    public int maxCollectibles = 4;
    
    public float respawnTime = 0.5f;
    private Vector2 screenBounds;

    // Use this for initialization
    void Start () {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        StartCoroutine(enemyWave());
    }

    private void addCollectibles() {
        for (int i = 0; i < maxCollectibles; i++)
        {
            //set random png
            GameObject collectible = Instantiate(collectiblePrefab) as GameObject;  
            SpriteRenderer spriteRenderer; 
            string spritePath = "Collectible";
            int randomCollNumber = i; //Random.Range(0, maxCollectibles - 1);
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
            collectible.transform.position = new Vector2(Random.Range(-screenBounds.x, screenBounds.x), Random.Range(-screenBounds.y, screenBounds.y));
        }
    }   

    private void spawnEnemy(){
        GameObject a = Instantiate(enemyPrefab) as GameObject;        
        a.transform.position = new Vector2(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y * 2);
    }

    IEnumerator enemyWave(){
        addCollectibles();
        while(true){
            yield return new WaitForSeconds(respawnTime);
            spawnEnemy();
        }
    }
}
