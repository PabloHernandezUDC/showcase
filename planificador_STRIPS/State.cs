public class State 
{
    // Obtiene o establece todos los predicados del estado.
    public List<Predicate> allPredicates { get; set; } = [];
    // Obtiene o establece todas las acciones pasadas del estado.
    public List<Action> pastActions { get; set; } = [];


    public State(List<Predicate> inputAllPredicates, List<Action> inputPastActions) {
        /*
        * Inicializa una nueva instancia de la clase State.
        *
        * Args:
        *    inputAllPredicates (List<Predicate>): Todos los predicados del estado.
        *    inputPastActions (List<Action>): Todas las acciones pasadas del estado.
        */
        allPredicates = inputAllPredicates;
        pastActions = inputPastActions;
    }

}