using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedNeuronalCapa
{
    public int numeroDeNodos, numeroDeNodosHijo, numeroDeNodosPadre;
    public float[,] pesos;
    public float[,] pesosIncremento;
    public float[] valoresNeuronas;
    public float[] valoresDeseados;
    public float[] errores;
    public float[] biasValores;
    public float[] biasPesos;

    public RedNeuronalCapa capaPadre, capaHija;

    public RedNeuronalCapa(int nNodosPadre, int nNodos, int nNodosHijo)
    {
        numeroDeNodos = nNodos;
        numeroDeNodosPadre = nNodosPadre;
        numeroDeNodosHijo = nNodosHijo;
        capaPadre = null;
        capaHija = null;
    }

    public void Inicializar(int nNodos, RedNeuronalCapa padre, RedNeuronalCapa hija)
    {
        numeroDeNodos = nNodos;

        valoresNeuronas = new float[nNodos];
        valoresDeseados = new float[nNodos];
        errores = new float[nNodos];

        if (padre != null)
        {
            capaPadre = padre;
            numeroDeNodosPadre = padre.numeroDeNodos;
        }
        else
        {
            capaPadre = null;
            numeroDeNodosPadre = 0;
        }

        if (hija != null)
        {
            capaHija = hija;
            numeroDeNodosHijo = hija.numeroDeNodos;
            pesos = new float[numeroDeNodos, numeroDeNodosHijo];
            pesosIncremento = new float[numeroDeNodos, numeroDeNodosHijo];
            biasValores = new float[numeroDeNodosHijo];
            biasPesos = new float[numeroDeNodosHijo];
        }
        else
        {
            capaHija = null;
            numeroDeNodosHijo = 0;
            pesos = null;
            pesosIncremento = null;
            biasValores = null;
            biasPesos = null;
        }

        for (int i = 0; i < numeroDeNodos; i++)
        {
            valoresNeuronas[i] = 0;
            valoresDeseados[i] = 0;
            errores[i] = 0;
        }

        if (hija != null)
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                for (int j = 0; j < numeroDeNodosHijo; j++)
                {
                    pesos[i, j] = 0;
                    pesosIncremento[i, j] = 0;
                }
            }

            for (int j = 0; j < numeroDeNodosHijo; j++)
            {
                biasValores[j] = -1;
                biasPesos[j] = 0;
            }
        }
    }

    public void AsignarPesosAleatorios()
    {
        if (capaHija != null)
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                for (int j = 0; j < numeroDeNodosHijo; j++)
                {
                    pesos[i, j] = Random.Range(-1f, 1f);
                }
            }

            for (int j = 0; j < numeroDeNodosHijo; j++)
            {
                biasPesos[j] = Random.Range(-1f, 1f);
            }
        }
    }

    public void CalcularValoresNeuronas()
    {
        if (capaPadre != null)
        {
            for (int j = 0; j < numeroDeNodos; j++)
            {
                float x = 0;
                for (int i = 0; i < numeroDeNodosPadre; i++)
                {
                    x += capaPadre.valoresNeuronas[i] * capaPadre.pesos[i, j];
                }
                x += capaPadre.biasValores[j] * capaPadre.biasPesos[j];

                if ((capaHija == null) && Constantes.OUTPUT_LINEAL)
                {
                    valoresNeuronas[j] = x;
                }
                else
                {
                    valoresNeuronas[j] = 1.0f / (1 + Mathf.Exp(-x)); // Función logística
                }
            }
        }
    }

    public void CalcularErrores()
    {
        if (capaHija == null)
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                // Ecuación del error
                errores[i] = (valoresDeseados[i] - valoresNeuronas[i]) * valoresNeuronas[i] * (1.0f - valoresNeuronas[i]);
            }
        }
        // ¿Es necesario este else if? Cuando esté funcionando, probar a quitarlo.
        else if (capaPadre == null)
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                errores[i] = 0.0f;
            }
        }
        else
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                float suma = 0;
                for (int j = 0; j < numeroDeNodosHijo; j++)
                {
                    suma += capaHija.errores[j] * pesos[i, j];
                }
                errores[i] = suma * valoresNeuronas[i] * (1.0f - valoresNeuronas[i]);
            }
        }
    }

    public void AjustarPesos()
    {
        if (capaHija != null)
        {
            for (int i = 0; i < numeroDeNodos; i++)
            {
                for (int j = 0; j < numeroDeNodosHijo; j++)
                {
                    // Comprobar si el error es el de capaHija.errores[j] o el de errores[i]
                    float dw = Constantes.RATIO_APRENDIZAJE * capaHija.errores[j] * valoresNeuronas[i];
                    if (Constantes.USO_INERCIA)
                    {
                        pesos[i, j] += dw + Constantes.RATIO_INERCIA * pesosIncremento[i, j];
                        pesosIncremento[i, j] = dw;
                    }
                    else
                    {
                        pesos[i, j] += dw;
                    }
                }
            }
            for (int j = 0; j < numeroDeNodosHijo; j++)
            {
                float dw = Constantes.RATIO_APRENDIZAJE * capaHija.errores[j] * biasValores[j];
                biasPesos[j] += dw;
            }
        }
    }
}
