abstract class Nodo {
    public abstract int Evaluar(int x = 0);
}
class NumeroNodo : Nodo
{
    public int Valor;
    public NumeroNodo(int valor)
    {
        Valor = valor;
    }
    public override int Evaluar(int x = 0)
    {
        return valor;
    }
}
class VariableNodo : Nodo
{
    public override int Evaluar(int x = 0)
    {
        return x;
    }

}
class SumaNodo : Nodo
{
    public Nodo Izquierdo;
    public Nodo Derecho;
    public  SumaNodo(Nodo izq,Nodo der)
    {
        Izquierdo=izq;
        Derecho=der;
    }
    public override int Evaluar(int x = 0)
    {
        return Izquierdo.Evaluar(x) + Derecho.Evaluar(x);
    }
}