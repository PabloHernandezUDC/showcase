public class Piece 
{
    // Obtiene o establece el nombre de la pieza.
    public string Name { get; set; } = string.Empty;
    // Obtiene o establece el valor de la pieza.
    public int Value { get; set; } = 0;
    
    public Piece(string inputName="", int inputValue=0) {
        /*
        * Inicializa una nueva instancia de la clase Piece.
        *
        * Args:
        *    inputName (string): El nombre de la pieza. Por defecto es una cadena vacía.
        *    inputValue (int): El valor de la pieza. Por defecto es 0.
        */
        Name = inputName;
        Value = inputValue;
    }
}