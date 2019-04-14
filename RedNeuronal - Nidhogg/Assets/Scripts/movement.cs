using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {
    public const float velocity = 0.1f;
    public bool grounded = true;
    [SerializeField] string controlHorizontal;
    [SerializeField] KeyCode UpCode, DownCode, attackCode;
    [SerializeField] Transform spawnPoint;
    public GameObject[] arms = new GameObject[3];

    private Color
        normal = new Color(1.0f, 1.0f, 1.0f, 1.0f),
        transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    //Outputs
    enum actions
    {
        attack      = 0, //ESPACIO
        moveTo      = 1, //ARROWLEFT
        moveAway    = 2, //ARROWRIGHT
        stay        = 3, //-
        swordUp     = 4, //ARROWUP
        swordDown   = 5, //ARROWDOWN
        jump        = 6  //E
    }
    //Estados de la espada
    enum stateArm
    {
        top = 0,
        mid = 1,
        bot = 2
    }
    stateArm actualstate = stateArm.mid;
    //\brief getActualState devuelve el estado actual
    public int getActualState()
    {
        return (int)actualstate;
    }
    void Start()
    {
        changeVisibility();
    }
    // Update is called once per frame
    void Update() {

        if (grounded)
        {
            if (Input.GetKeyDown(attackCode))
            {
                StartCoroutine(Attack());
            }
            //DESPLAZAMIENTO
            transform.Translate
                (
                    Input.GetAxis(controlHorizontal) * velocity,
                    0,
                    0
                );

            //SUBIR ARMA
            if (Input.GetKeyDown(UpCode))
            {
                if (actualstate != stateArm.top)
                {
                    actualstate--;
                    changeVisibility();
                }

            }
            //BAJAR ARMA
            else if (Input.GetKeyDown(DownCode))
            {
                if (actualstate != stateArm.bot)
                {
                    ++actualstate;
                    changeVisibility();
                }
            }
        }
    }
    //\brief changeVisibility dependiendo del estado de la espada cambia la visibilidad de los brazos
    private void changeVisibility()
    {
        switch (actualstate)
        {
            case stateArm.top:
                activateArm(arms[0]);
                deactivateArm(arms[1]);
                deactivateArm(arms[2]);
                break;
            case stateArm.mid:
                deactivateArm(arms[0]);
                activateArm(arms[1]);
                deactivateArm(arms[2]);
                break;
            case stateArm.bot:
                deactivateArm(arms[0]);
                deactivateArm(arms[1]);
                activateArm(arms[2]);
                break;
        }
    }
    //\brief activateArm activa el color del brazo y su collider
    //@param arm brazo del pawn
    private void activateArm(GameObject arm)
    {
        arm.GetComponent<SpriteRenderer>().color = normal;
        arm.GetComponent<BoxCollider2D>().enabled = true;
    }
    //\brief deactivateArm desactiva el color del brazo y su collider
    //@param arm brazo del pawn
    private void deactivateArm(GameObject arm)
    {
        arm.GetComponent<SpriteRenderer>().color = transparent;
        arm.GetComponent<BoxCollider2D>().enabled = false;
    }

    //\brief Attack el personaje ataca
    IEnumerator Attack()
    {
        //Se le limitan las acciones y se deplaza el brazo hacia delante
        grounded = false;
        int direction = 1;
        if (attackCode == KeyCode.Space)
            direction = -1;
        arms[(int)actualstate].transform.position += new Vector3
            (
                -0.8f * direction,
                0.0f,
                0.0f
            );
        //CD del ataque
        yield return new WaitForSeconds(0.2f);
        arms[(int)actualstate].transform.position += new Vector3
            (
                0.8f * direction,
                0.0f,
                0.0f
            );
        grounded = true;
    }

    //\brief getSpawnPoint devuelve el spawn point del personaje
    public Transform getSpawnPoint()
    {
        return spawnPoint;
    }
    //\brief setGrounded setea el valor de grounded
    public void setGrounded(bool state)
    {
        grounded = state;
    }
    
    //cuando entra en colision con el suelo se setea grounded a true
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
            grounded = true;
    }
    //\brief getGroundedValue devuelve el valor de grounded en valor integer para trabajar con  la red neuronal
    public int getGroundedValue()
    {
        if (grounded)
            return 0;
        else
            return 1;
    }


}
