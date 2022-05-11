using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemys : MonoBehaviour
{
    public Enemy[] prefabs;//used for each row
    [SerializeField] private int rows = 5;//rows of enemys, typically there are 5
    [SerializeField] private int columns = 11;//columns of enemys, typically there are 11
    [SerializeField] private float EnemySpacing = 2;//seperation space 
    [SerializeField] private float enemyAttackRate = 0.4f;// interval of attacks
    [SerializeField] private Text scoreLabel;
    public AnimationCurve EnemySpeed;//x,y graph with x being % killed and y being speed
    public float speedMultiplier = 1.0f;
    public Bomb bombPrefab;

    private Vector3 direction = Vector2.right;
    public int enemyTotal => this.rows * this.columns;//for calculations of attack&move speed
    public int enemyTotalAlive => this.enemyTotal - this.killCount;//for calculation attack speed
    public int killCount { get; private set; }//half public half private
    public float percentKilled => (float)this.killCount / (float)this.enemyTotal;//used for game speed

    private int score;


    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float totalWidth = EnemySpacing * (this.columns - 1);
            float totalHeight = EnemySpacing * (this.rows - 1);
            Vector2 center = new Vector2(-totalWidth / 2, -totalHeight / 2);
            Vector3 rowPos = new Vector3(center.x, center.y + (row * EnemySpacing), 0.0f);//need position of the row to offset the column

            for (int column = 0; column < this.columns; column++)
            {
                //Instantiate each enemy which will be which ever we are on.
                Enemy enemy = Instantiate(this.prefabs[row], this.transform);
                enemy.killed += EnemyKilled;
                Vector3 position = rowPos;//set the position of invader to row
                position.x += column * EnemySpacing;//offsetting the enemys
                enemy.transform.localPosition = position;//relative to parent positioning
            }
        }

        score = 0;
        scoreLabel.text = $"Score: {score}";
    }

    private void Start()
    {
        InvokeRepeating(nameof(enemyAttack), this.enemyAttackRate, this.enemyAttackRate);//invokes enemy attack every set interval
    }

    private void Update()//every frame
    {



        this.transform.position += this.EnemySpeed.Evaluate(this.percentKilled) * Time.deltaTime * direction * speedMultiplier;//consistent movement regardless of fps

        Vector3 LEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);//translate view port cordinates to world cordinates
        Vector3 REdge = Camera.main.ViewportToWorldPoint(Vector3.right);//translate view port cordinates to world cordinates

        foreach (Transform enemy in this.transform)//loops through all child objects attached to this
        {
            if (!enemy.gameObject.activeInHierarchy)//if enemy is active or disabled if not active its killed
            {
                continue;
            }
            //checks if enemys have hit the edge then changes direction and decend a row
            if (direction == Vector3.right && enemy.position.x >= REdge.x - 1.0f)//-1 for padding so they dont exceed screen
            {
                DecendRow();
            }
            //checks if enemys have hit the edge then changes direction and decend a row
            else if (direction == Vector3.left && enemy.position.x <= LEdge.x + 1.0f)//1 for padding so they dont exceed screen
            {
                DecendRow();
            }
        }
    }
    private void enemyAttack()
    {
        foreach (Transform enemy in this.transform)//loops through all child objects attached to this
        {
            if (!enemy.gameObject.activeInHierarchy)//if enemy is active or disabled if not active its killed
            {
                continue;
            }
            //random is a value between 0 and 1, the more enemys killed the higher chance the alive enemys will shoot
            if (Random.value < (1.0f / (float)this.enemyTotalAlive))
            {
                Instantiate(this.bombPrefab, enemy.position, Quaternion.identity);
                break;//only one missile can be launched at a time
            }
        }
    }
    private void DecendRow()
    {
        direction.x *= -1.0f;// travel to opposite side of screen
        Vector3 position = this.transform.position;//get current position
        position.y -= 1.0f;//decend enemy
        this.transform.position = position;//assign back to transform
    }

    private void EnemyKilled()
    {
        this.killCount++;
        UpdateScore(10);
        if (enemyTotalAlive == 0)
        {
            StartCoroutine(Dead());
        }
    }

    private void UpdateScore(int value)
    {
        score += value;
        scoreLabel.text = $"Score: {score}";
    }

    private IEnumerator Dead()
    {
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(0);
    }

}
