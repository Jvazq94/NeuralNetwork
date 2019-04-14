using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RedNeuronal
{
    private RedNeuronalCapa capaEntrada, capaOculta, capaSalida;

    public RedNeuronal(int nNodosInput, int nNodosOcultos, int nNodosOutput)
    {
        capaEntrada = new RedNeuronalCapa(            0,   nNodosInput, nNodosOcultos);
        capaOculta  = new RedNeuronalCapa(  nNodosInput, nNodosOcultos,  nNodosOutput);
        capaSalida  = new RedNeuronalCapa(nNodosOcultos,  nNodosOutput,             0);

        capaEntrada.Inicializar(   nNodosInput,        null, capaOculta);
        capaOculta.Inicializar ( nNodosOcultos, capaEntrada, capaSalida);
        capaSalida.Inicializar (  nNodosOutput,  capaOculta,       null);

        capaEntrada.AsignarPesosAleatorios();
        capaOculta.AsignarPesosAleatorios();
    }

    public void SetInput(int i, float valor)
    {
        if ((i >= 0) && (i < capaEntrada.numeroDeNodos))
        {
            capaEntrada.valoresNeuronas[i] = valor;
        }
    }

    public float GetOutput(int i)
    {
        if ((i >= 0) && (i < capaSalida.numeroDeNodos))
        {
            return capaSalida.valoresNeuronas[i];
        }
        else
        {
            return Constantes.ERROR_SALIDA;
        }
    }

    public void SetOutputDeseado(int i, float valor)
    {
        if ((i >= 0) && (i < capaSalida.numeroDeNodos))
        {
            capaSalida.valoresDeseados[i] = valor;
        }
    }

    public void FeedForward()
    {
        capaEntrada.CalcularValoresNeuronas();
        capaOculta.CalcularValoresNeuronas();
        capaSalida.CalcularValoresNeuronas();
    }

    public void BackPropagation()
    {
        capaSalida.CalcularErrores();
        capaOculta.CalcularErrores();

        capaOculta.AjustarPesos();
        capaEntrada.AjustarPesos();
    }

    public int GetMaxOutputId()
    {
        int id = -1;
        float max = float.MinValue;

        for (int i = 0; i < capaSalida.numeroDeNodos; i++)
        {
            if (capaSalida.valoresNeuronas[i] > max)
            {
                max = capaSalida.valoresNeuronas[i];
                id = i;
            }
        }

        return id;
    }

    public float CalcularError()
    {
        float error = 0;

        for (int i = 0; i < capaSalida.numeroDeNodos; i++)
        {
            error += Mathf.Pow((capaSalida.valoresNeuronas[i] - capaSalida.valoresDeseados[i]), 2);
        }

        error /= capaSalida.numeroDeNodos;
        return error;
    }

}