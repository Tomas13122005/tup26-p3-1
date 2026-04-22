class Compilador {
    public static Nodo Parse(string expresion) {
        throw new NotImplementedException("Implementar el parser para convertir la expresión en un AST.");
    }

    private string texto;
    private int posicion;
    private Token tokenActual;

    private Compilador(string texto) {
        this.texto = texto;
        this.posicion = 0;
        this.tokenActual = LeerToken();
    }

    private Token LeerToken() {
    while (posicion < texto.Length && texto[posicion] == ' ') posicion++;

    if (posicion >= texto.Length) return new Token(TipoToken.Fin);

    char c = texto[posicion++];
    return c switch {
        '+' => new Token(TipoToken.suma),
        '-' => new Token(TipoToken.resta),
        '*' => new Token(TipoToken.multiplicacion),
        '/' => new Token(TipoToken.division),
        '(' => new Token(TipoToken.parentesisAbierto),
        ')' => new Token(TipoToken.parentesisCerrado),
        'x' or 'X' => new Token(TipoToken.variable),
        _ when char.IsDigit(c) => LeerNumero(c),
        _ => throw new FormatException($"Token inesperado: '{c}'")
    };
}

private Token LeerNumero(char primero) {
    var sb = new StringBuilder();
    sb.Append(primero);
    while (posicion < texto.Length && char.IsDigit(texto[posicion]))
        sb.Append(texto[posicion++]);
    return new Token(TipoToken.Numero, int.Parse(sb.ToString()));
}
}


enum TokenTipo {
    numero,
    variable,
    suma,
    resta,
    multiplicacion,
    division,
    parentesisAbierto,
    parentesisCerrado,
    finExpresion
}

record Token(TipoToken Tipo, int Valor = 0);



