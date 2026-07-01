using UnityEngine;

public class DebugScript : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private bool killAllPressed;

    private bool killOnePressed;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        killAllPressed = false;

        killOnePressed = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.KillAll.performed += ctx =>
        {
            killAllPressed = true;

        };
        inputActions.Player.KillOne.performed += ctx =>
        {
            killOnePressed = true;
        };
    }
    private void OnDisable()
    {
        inputActions.Disable();

        Debug.Log("Disabled");
    }

    private void Update()
    {
        if (killAllPressed)
        {
            KillAllEnemies();
            killAllPressed = false;
        }

        if (killOnePressed)
        {
            KillOneEnemy();
            killOnePressed = false;
        }
        
    }


    public void KillOneEnemy()
    {
        GameObject enemyObject =
            GameObject.FindGameObjectWithTag("Enemy");

        if (enemyObject == null)
            return;

        Enemy enemy =
            enemyObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Kill();
        }
    }

    public void KillAllEnemies()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy =
                enemyObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.Kill();
            }
        }
    }
}
