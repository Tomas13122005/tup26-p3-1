
class Compilador 
{
    public static Nodo Parse(string expresion) 
    {
     var parser = new Parser(expresion);
     return parser.Parsear();
    }
}
class Parser
{
    private string texto;
    private int pos;
    public Parser (string texto)
    {
        this.texto = texto.Replace(" ", "");
        this.pos = 0;
    }
    public  Nodo Parsear()
    {
        return ParseExpresion();
    }
}