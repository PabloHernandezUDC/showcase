public class Predicate 
{
    // Obtiene o establece el nombre del predicado.
    public string Name { get; set; } = string.Empty;
    // Obtiene o establece los miembros del predicado.
    public Piece[] Members { get; set;} = [];
    // Obtiene o establece el estado del predicado.
    public bool State { get; set; } = false;

    public Predicate(string inputName, Piece[] inputMembers, bool inputState=false) {
        /*
        * Inicializa una nueva instancia de la clase Predicate.
        *
        * Args:
        *    inputName (string): El nombre del predicado.
        *    inputMembers (Piece[]): Los miembros del predicado.
        *    inputState (bool): El estado del predicado. Por defecto es falso.
        */
        Name = inputName;
        Members = inputMembers;
        State = inputState;
    }
    
}