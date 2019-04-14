using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemeyMovement : MonoBehaviour {
    public float velocity = 0.1f;
    public bool grounded = true;
    [SerializeField] KeyCode UpCode, DownCode, attackCode;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] arms = new GameObject[3];
    [SerializeField] GameObject myEnemy;
    public float[] inputs = new float[3];
    public float[] output = new float[7];
    public RedNeuronalCharacter cerebro;
    public float rango = 0.5f;
    public int kills = 0;
    public int deaths = 0;
    public Text killsTxt;
    public Text deathTxt;
    public bool passive;
    bool flag = false;
    bool llamar = true;

   //Outputs
   public enum actions
    {
        attack      = 0, //ESPACIO
        moveTo      = 1, //ARROWLEFT
        moveAway    = 2, //ARROWRIGHT
        stay        = 3, //-
        swordUp     = 4, //ARROWUP
        swordDown   = 5, //ARROWDOWN
        jump        = 6  //E
    }
    public actions currentAction = actions.stay;
    //Estado espada
    enum stateArm
    {
        top = 0,
        mid = 1,
        bot = 2
    }
    stateArm actualstate = stateArm.mid;
    private Color
        normal = new Color(1.0f, 1.0f, 1.0f, 1.0f),
        transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    // Use this for initialization
    void Start () {
        changeVisibility();//Se activa el brazo correspondiente
        if (!passive)
        {
            //Se setean los textos
            killsTxt.text = "Kills: " + kills;
            deathTxt.text = "Kills: " + deaths;
        }
        else
        {
            velocity = -velocity;
           
        }

       

    }
    
    // Update is called once per frame
    void Update () {
        if (grounded)
        {
            //Se decide la accion segun los inputs
            decideAction();
            
            //Se realiza la accion decidida
            if (currentAction != actions.stay)
            {
                switch (currentAction)
                {
                    case actions.moveAway:
                        transform.Translate(1 * velocity,0,0);
                        break;
                    case actions.moveTo:
                        transform.Translate(-1 * velocity, 0, 0);
                        break;
                    case actions.swordDown:
                        if (actualstate != stateArm.bot)
                        {
                            ++actualstate;
                            changeVisibility();
                        }
                        break;
                    case actions.swordUp:
                        if (actualstate != stateArm.top)
                        {
                            actualstate--;
                            changeVisibility();
                        }
                        break;
                    case actions.attack:                        
                        if(grounded && !flag)                        
                            StartCoroutine(Attack());                        

                        break;
                    case actions.jump:
                        break;
                }
                
                
            }
            if (!passive)
            {
                //Si la ia tiene activado el aprendizaje se realiza un refuerzo
                Refuerzo();
            }

            for (int i = 0; i < output.Length; i++)
            {
                //Se setean los outpus por tick para poder verlos en tiempo real
                output[i] = cerebro.GetOutput(i);
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
    //\brief Attack la IA ataca
    IEnumerator Attack()
    {
        //Solo ataca cuando esta  en tierra 
        if (grounded && !flag)
        {
            //Se le limitan las acciones y se deplaza el brazo hacia delante
            flag = true;
            grounded = false;
            int direction = 1;
            if (attackCode == KeyCode.Space || passive)
                direction = -1;
            foreach (GameObject auto in arms)
            {
                auto.transform.position += new Vector3
                (
                    -rango * direction,
                    0.0f,
                    0.0f
                );
            }
            //CD del ataque
            yield return new WaitForSeconds(0.3f);
            foreach (GameObject auto in arms)
            {
                auto.transform.position += new Vector3
                (
                    rango * direction,
                    0.0f,
                    0.0f
                );
            }

            yield return new WaitForSeconds(1.0f);
            grounded = true;
            flag = false;
            StopCoroutine(Attack());
        }
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

    //\brief dediceAction llama a la red neuronal apra decidir la accion
    void decideAction()
    {
        //    enum actions
        //{
        //    attack = 0, //ESPACIO
        //    moveTo = 1, //ARROWLEFT
        //    moveAway = 2, //ARROWRIGHT
        //    stay = 3, //-
        //    swordUp = 4, //ARROWUP
        //    swordDown = 5, //ARROWDOWN
        //    jump = 6  //E
        //}
        //INPUTS 
        //  0 =distancia   ;   1=estadoEspada  ;   2=pegando
        //Se setean los inputs segun los valores de mi oponente
        inputs[0] = Vector3.Distance(myEnemy.transform.position, transform.position);
        movement script = myEnemy.GetComponent<movement>();
        enemeyMovement enemyscr = myEnemy.GetComponent<enemeyMovement>();
        if (script != null)
        {
            inputs[1] = myEnemy.GetComponent<movement>().getActualState();
            inputs[2] = myEnemy.GetComponent<movement>().getGroundedValue();
        }
        else if(enemyscr != null)
        {
            inputs[1] = (int)myEnemy.GetComponent<enemeyMovement>().actualstate;
            if (myEnemy.GetComponent<enemeyMovement>().grounded)
                inputs[2] = 0;
            else
                inputs[2] = 1;
        }
        actions nuevoEstado = actions.stay;
        nuevoEstado = (actions)cerebro.ConsultarAccion(inputs);  
        currentAction = nuevoEstado;
        
    }

    //\brief Refuerzo se refuerza varias acciones con unos imputs determinado para ayudar a la red neuronal
    private void Refuerzo()
    {
        
        // Attack ; MoveTo ; MoveAway ; Stay ; SwordUp ; SwordDown ; Jump
        float distancia = Vector3.Distance(transform.position, myEnemy.transform.position);
        
        //Cuando la distancia es mayor que 5 reforzamos que la espada este en el centro y la IA avance
        if (distancia > 5.0f)
        {
            if (actualstate == stateArm.bot)
            {
                output = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.5f, 0.1f, 0.1f };
                cerebro.ReentrenarRed(inputs, output);
            }
            else if (actualstate == stateArm.top)
            {
                output = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.5f, 0.1f };
                cerebro.ReentrenarRed(inputs, output);
            }
            else
            {
                // REFUERZO AL MOVETO
                output = new float[] { 0.1f, 0.9f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
                cerebro.ReentrenarRed(inputs, output);
            }
        }

        //A media distancia reforzamos un cambio de espada para poder atacar
        else if (distancia < 5.0f && distancia > 3.0f)
        {
            if (inputs[2] == 0)
            {
                if (inputs[1] == (int)actualstate)
                {
                    output = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.6f, 0.8f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
                else
                {
                    output = new float[] { 0.4f, 0.7f, 0.1f, 0.1f, 0.4f, 0.5f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
            }
   
        }
        //Cuando la distancia es pequeña reforzamos el ataque dependiedno de la posicion de la espada del enemigo
        else if (distancia < 3.0f)
        {
            if (inputs[2] == 0)
            {
                if (inputs[1] != (int)actualstate)
                {
                    output = new float[] { 0.6f, 0.1f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
                else
                {
                    output = new float[] { 0.4f, 0.2f, 0.1f, 0.1f, 0.6f, 0.4f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
            }
            else if (inputs[2] == 1)
            {
                if (inputs[1] != (int)actualstate)
                {
                    output = new float[] { 0.3f, 0.2f, 0.6f, 0.1f, 0.1f, 0.1f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
                else
                {
                    output = new float[] { 0.4f, 0.5f, 0.6f, 0.1f, 0.1f, 0.1f, 0.1f };
                    cerebro.ReentrenarRed(inputs, output);
                }
                
            }
        }
        
    }
}
