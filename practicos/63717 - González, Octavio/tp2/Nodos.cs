using System;

namespace Calculadora
{
    // Esta es la clase padre de todos los nodos
    public abstract class Nodo
    {
        // Todos los nodos tienen que tener este metodo para sacar la cuenta
        // Si no le pasamos x, vale 0 por defecto
        public abstract int Evaluar(int x = 0);
    }

    // Este nodo es para cuando hay un numero solo, como el 5 o el 10
    public class NumeroNodo : Nodo
    {
        public int MiNumero;

        public NumeroNodo(int numero)
        {
            MiNumero = numero;
        }

        public override int Evaluar(int x)
        {
            // Aca solo devolvemos el numero que guardamos
            return MiNumero;
        }
    }

    // Este es para cuando aparece la letra x en la cuenta
    public class VariableNodo : Nodo
    {
        public override int Evaluar(int x)
        {
            // Devolvemos el valor de x que nos pasaron
            return x;
        }
    }
}
