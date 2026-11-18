
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TipoObjetoInventario
{
    Botella,
    Lata,
    ResiduosBarrido,
    Escombros,
    Asfalto
}

public class InventarioJugador : MonoBehaviour
{
    [Header("Capacidad de la mochila")]
    [SerializeField, Min(1)]
    private int capacidadMaxima = 5;

    private Dictionary<TipoObjetoInventario, int> objetos =
        new Dictionary<TipoObjetoInventario, int>();

    public int EspaciosOcupados { get; private set; }

    public int CapacidadMaxima => capacidadMaxima;

    public int EspaciosDisponibles =>
        Mathf.Max(0, capacidadMaxima - EspaciosOcupados);

    public event Action InventarioActualizado;

    public int ObtenerCantidad(TipoObjetoInventario tipo)
    {
        if (objetos.TryGetValue(tipo, out int cantidad))
            return cantidad;

        return 0;
    }

    public bool AgregarObjeto(
        TipoObjetoInventario tipo,
        int cantidad = 1)
    {
        if (cantidad <= 0 || cantidad > EspaciosDisponibles)
        {
            Debug.Log("Mochila llena o cantidad invalida.");
            return false;
        }

        objetos[tipo] = ObtenerCantidad(tipo) + cantidad;

        EspaciosOcupados += cantidad;

        InventarioActualizado?.Invoke();

        Debug.Log(
            "Recogiste " + cantidad + " " + tipo +
            ". Mochila: " + EspaciosOcupados +
            "/" + capacidadMaxima
        );

        return true;
    }

    public bool QuitarObjeto(
        TipoObjetoInventario tipo,
        int cantidad = 1)
    {
        if (cantidad <= 0 || ObtenerCantidad(tipo) < cantidad)
            return false;

        int restante = ObtenerCantidad(tipo) - cantidad;

        if (restante == 0)
            objetos.Remove(tipo);
        else
            objetos[tipo] = restante;

        EspaciosOcupados -= cantidad;

        InventarioActualizado?.Invoke();

        return true;
    }

    public void EstablecerCapacidadMaxima(int nuevaCapacidad)
    {
        capacidadMaxima = Mathf.Max(EspaciosOcupados, nuevaCapacidad);
        InventarioActualizado?.Invoke();
    }
    
    [ContextMenu("Prueba - Agregar botella")]
    private void ProbarInventario()
    {
        AgregarObjeto(TipoObjetoInventario.Botella);
    }
}
