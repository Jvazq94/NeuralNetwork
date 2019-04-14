using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionSword : MonoBehaviour {
    public GameObject father;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //\brief OnTriggerEnter2D comprueba la colision entre las espadas
    //@param collision collider con el que colisiona
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Comprueba si la colision es una espada
        if (collision.gameObject.CompareTag("sword"))
        {
            //Ponemos la variable grounded en false para que tanto el player como la IA no puedan realizar ninguna accion

            movement script = transform.parent.transform.parent.gameObject.GetComponent<movement>();
            if (script != null)
            {
                script.setGrounded(false);
            }
            enemeyMovement enemyscr = transform.parent.transform.parent.gameObject.GetComponent<enemeyMovement>();
            if (enemyscr != null)
            {
                enemyscr.setGrounded(false);
            }

            //Desplazamos los pawn añadiendole una fuerza en el lado contrario al que golpean
            Vector3 dir = collision.gameObject.transform.position - transform.position;
            if (dir.x > 0)
            {
                transform.parent.transform.parent.gameObject.GetComponent<Rigidbody2D>().AddForce
                (
                    new Vector3
                    (
                        -100.0f,
                        +100.0f
                    )
                );
            }
            else
            {
                transform.parent.transform.parent.gameObject.GetComponent<Rigidbody2D>().AddForce
                (
                    new Vector3
                    (
                        100.0f,
                        100.0f
                    )
                );
            }
        }

        //Comprobamos si la colision de la espada es con un character(tanto IA como jugador)
        if (collision.gameObject.CompareTag("character"))
        {

            movement script = collision.gameObject.GetComponent<movement>();
            
            enemeyMovement enemyscr  = collision.gameObject.GetComponent<enemeyMovement>();
            enemeyMovement abuelo = transform.parent.transform.parent.gameObject.GetComponent<enemeyMovement>();
            movement abueloPlayer = transform.parent.transform.parent.gameObject.GetComponent<movement>();

            //Comprobamos que no choque contra si mismo
            if (collision.gameObject != transform.parent.transform.parent.gameObject) {
                
                //cuando la IA mata al jugador
                if (script != null)
                {
                    //Teletransportamos a los dos characters a su respectivo spawn point
                    collision.gameObject.transform.position = script.getSpawnPoint().position;
                    transform.parent.transform.parent.gameObject.transform.position = abuelo.getSpawnPoint().position;
                    abuelo.kills++;
                    abuelo.killsTxt.text = "Kills: " + abuelo.kills;

                    //Debido a que la IA ha matado reforzamos la ultima accion realizada
                    if (abuelo.output[(int)abuelo.currentAction] < 0.9f)
                    {
                        Debug.Log("la ia aprende");
                        abuelo.output[(int)abuelo.currentAction] = abuelo.output[(int)abuelo.currentAction] + 0.1f;
                        abuelo.cerebro.ReentrenarRed(abuelo.inputs, abuelo.output);
                    }

                }
                //Cuando una IA mata otra IA
                if (enemyscr != null)
                {
                    collision.gameObject.transform.position = enemyscr.getSpawnPoint().position;

                    //Mueve los characters a su respectivos spawn point

                    if (abuelo != null)
                    {
                        transform.parent.transform.parent.gameObject.transform.position = transform.parent.transform.parent.gameObject.GetComponent<enemeyMovement>().getSpawnPoint().position;
                    }
                    else
                    {
                        transform.parent.transform.parent.gameObject.transform.position = transform.parent.transform.parent.gameObject.GetComponent<movement>().getSpawnPoint().position;
                    }

                    if (abuelo != null)
                    {

                        //Comprueba si la IA es pasiva(no aprende)
                        if (abuelo.passive)
                        {   //La IA sin red neuronal mata
                            //El enemigo cuenta las kills de la IA pasiva
                            enemyscr.deaths++;
                            enemyscr.deathTxt.text = "Kills: " + enemyscr.deaths;
                            //Entrenamiento IA lista
                            //La IA con la red neuronal muere y se refuerza negativamente la ultima accion
                            if (enemyscr.output[(int)enemyscr.currentAction] > 0.1f)
                            {
                                Debug.Log("La ia desaprende");
                                float aux = enemyscr.output[(int)enemyscr.currentAction] - .1f;
                                if (aux < 0.1f)
                                {
                                    enemyscr.output[(int)enemyscr.currentAction] = .1f;
                                }
                                else
                                {
                                    enemyscr.output[(int)enemyscr.currentAction] = enemyscr.output[(int)enemyscr.currentAction] - .1f;
                                }
                                enemyscr.cerebro.ReentrenarRed(enemyscr.inputs, enemyscr.output);
                            }
                            ////Entrenamiento IA tonta
                            //if (abuelo.output[(int)abuelo.currentAction] < 0.9f)
                            //{
                            //    Debug.Log("la tonta aprende");
                            //    float aux = abuelo.output[(int)abuelo.currentAction] + 0.2f;
                            //    if (aux > 0.9f)
                            //        abuelo.output[(int)abuelo.currentAction] = 0.9f;
                            //    else
                            //        abuelo.output[(int)abuelo.currentAction] = abuelo.output[(int)abuelo.currentAction] + 0.2f;                              
                               
                            //    abuelo.cerebro.ReentrenarRed(abuelo.inputs, abuelo.output);
                            //}
                        }
                        else
                        {
                            //La IA con red neuronal mata
                            //Cuenta sus propias kills
                            abuelo.kills++;
                            abuelo.killsTxt.text = "Kills: " + abuelo.kills;
                            //Entrenamiento IA lista
                            //Se refuerza positivamente la ultima accion realizada
                            if (abuelo.output[(int)abuelo.currentAction] < 0.8f)
                            {
                                Debug.Log("la ia aprende");
                                float aux = abuelo.output[(int)abuelo.currentAction] + 0.2f;

                                if (aux > 0.9f)
                                    abuelo.output[(int)abuelo.currentAction] = 0.9f;
                                else
                                    abuelo.output[(int)abuelo.currentAction] = abuelo.output[(int)abuelo.currentAction] + 0.2f;

                                abuelo.cerebro.ReentrenarRed(abuelo.inputs, abuelo.output);
                            }

                            ////Entrenamiento IA tonta
                            //Debug.Log(enemyscr.output[(int)enemyscr.currentAction]);
                            //if (enemyscr.output[(int)enemyscr.currentAction] > 0.1f)
                            //{
                            //    Debug.Log("la tonta desaprende");
                            //    float aux = enemyscr.output[(int)enemyscr.currentAction] - .1f;
                            //    if (aux < 0.1f)
                            //    {
                            //        enemyscr.output[(int)enemyscr.currentAction] = .1f;
                            //    }
                            //    else
                            //    {
                            //        enemyscr.output[(int)enemyscr.currentAction] = enemyscr.output[(int)enemyscr.currentAction] - .1f;

                            //    }
                            //    enemyscr.cerebro.ReentrenarRed(enemyscr.inputs, enemyscr.output);
                            //}

                        }
                    }
                    else
                    {
                        //Un player mata a una IA con red neuronal
                        enemyscr.deaths++;
                        enemyscr.deathTxt.text = "Kills: " + enemyscr.deaths;

                        //Se refuerza negativamente la ultima accion realizada por la IA
                        if (enemyscr.output[(int)enemyscr.currentAction] > 0.1f)
                        {
                            Debug.Log("La ia desaprende");
                            float aux = enemyscr.output[(int)enemyscr.currentAction] - .1f;
                            if (aux < 0.1f)
                            {
                                enemyscr.output[(int)enemyscr.currentAction] = .1f;
                            }
                            else
                            {
                                enemyscr.output[(int)enemyscr.currentAction] = enemyscr.output[(int)enemyscr.currentAction] - .1f;
                            }
                            enemyscr.cerebro.ReentrenarRed(enemyscr.inputs, enemyscr.output);
                        }
                    }



                }

            }
        }

    }
}
