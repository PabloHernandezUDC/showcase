public class Planner 
{
    
    public State CurrentState { get; set; }

    public List<State> Goals { get; set; }



    public Planner(State inputCurrentState, List<State> inputGoal) {
        /*
        * Inicializa una nueva instancia de la clase Planner.
        *
        * Args:
        *    inputCurrentState (State): El estado actual del planificador.
        *    inputGoal (List<State>): Una lista de estados objetivo para el planificador.
        */
        CurrentState = inputCurrentState;
        Goals = inputGoal;
    }



    public bool comparePieceLists(Piece[] l1, Piece[] l2) {
        /*
        * Compara dos listas de piezas.
        *
        * Args:
        *    l1 (Piece[]): La primera lista de piezas.
        *    l2 (Piece[]): La segunda lista de piezas.
        *
        * Returns:
        *    bool: Verdadero si las dos listas son iguales, falso en caso contrario.
        */

        // comparamos por orden las listas de piezas
        // y si en algún momento hay alguna diferencia, devolvemos false
        // si no, true
        for (int i = 0; i < l1.Length; i++) {
            if (!(l1[i].Name == l2[i].Name)) {
                return false;
            }
        }
        return true;
    }



    public bool isContainedIn(State small, State big) {
        /*
        * Comprueba si un estado está contenido en otro.
        *
        * Args:
        *    small (State): El estado que se va a comprobar.
        *    big (State): El estado en el que se va a buscar.
        *
        * Returns:
        *    bool: Verdadero si el estado 'small' está contenido en 'big', falso en caso contrario.
        */
        
        // para cada predicado del small, miramos si hay uno igual en el big
        // si en algún momento hay un predicado que no encontramos, devolvemos false
        // si llega hasta el final sin problemas, devolvemos true
        foreach (Predicate p1 in small.allPredicates) {
            bool found = false;
            foreach (Predicate p2 in big.allPredicates) {
                if (p1.Name == p2.Name && comparePieceLists(p1.Members, p2.Members)) {                    
                    found = true;
                    break;
                }
            }
            if (!found) {
                return false;
            }
        }
        return true;
    }



    public bool isContainedIn(State small, List<State> big) {
        /*
        * Comprueba si un estado está contenido en una lista de estados.
        *
        * Args:
        *    small (State): El estado que se va a comprobar.
        *    big (List<State>): La lista de estados en la que se va a buscar.
        *
        * Returns:
        *    bool: Verdadero si el estado 'small' está contenido en 'big', falso en caso contrario.
        */
        
        // miramos todos los estados de big
        // y si en algún momento hay una coincidencia, devolvemos true
        // y si hemos llegado al final y no hemos encontrado una coincidencia, devolvemos false
        foreach (State s in big) {
            if (isContainedIn(small, s)) {
                return true;
            }
        }
        return false;
    }



    public bool CheckGoal(State state) {
        /*
        * Comprueba si un estado es un estado objetivo.
        *
        * Args:
        *    state (State): El estado que se va a comprobar.
        *
        * Returns:
        *    bool: Verdadero si el estado es un estado objetivo, falso en caso contrario.
        */

        // miramos todas las metas
        // y si en algún momento hay una coincidencia, devolvemos true
        // y si hemos llegado al final y no hemos encontrado una coincidencia, devolvemos false
        foreach (State goal in Goals) {
            if (isContainedIn(goal, state)) {
                return true;
            }
        }
        return false;
    }



    public void printState(State s) {
        /*
        * Imprime un estado.
        *
        * Args:
        *    s (State): El estado que se va a imprimir.
        */
        foreach (Predicate p in s.allPredicates) {
            Console.Write(p.Name + " ");
            foreach (Piece p2 in p.Members) {
                Console.Write(p2.Name + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }



    public virtual List<Action> GenerateActions(State state) {
        /*
        * Genera las acciones posibles para un estado.
        *
        * Args:
        *    state (State): El estado para el que se van a generar las acciones.
        *
        * Returns:
        *    List<Action>: Una lista de acciones posibles para el estado.
        */
        
        // para cada bloque libre, moverlo a todos los demás bloques libres
        // y si no está encima de la mesa, moverlo a la mesa
        List<Action> resultActions = [];

        foreach (Predicate currentPredicate1 in state.allPredicates) {
            if (currentPredicate1.Name == "Free" && currentPredicate1.Members[0].Name != "Table") {
                // extraemos la pieza actual
                Piece currentPiece = currentPredicate1.Members[0];
                Piece pieceBelow = null;
                // encontrar la pieza que está debajo
                foreach (Predicate currentPredicate2 in state.allPredicates) {
                    if (currentPredicate2.Name == "On" && currentPredicate2.Members[0] == currentPiece) {
                        pieceBelow = currentPredicate2.Members[1];
                        break;
                    }
                }
                // moverlo a todos los demás bloques libres
                foreach (Predicate currentPredicate2 in state.allPredicates) {
                    // si encontramos otra que está libre
                    if (currentPredicate2.Name == "Free" && currentPredicate2.Members[0].Name != "Table" && currentPredicate1 != currentPredicate2 && pieceBelow != null) {
                        resultActions.Add(new MovePiece([currentPiece, pieceBelow, currentPredicate2.Members[0]]));
                    }
                }
                // moverlo a la mesa, si no lo está ya
                // primero comprobamos si está en la mesa
                if (pieceBelow != null) {
                    // si NO está encima de la mesa
                    if (pieceBelow.Name != "Table") {
                        resultActions.Add(new MovePieceToTable([currentPiece, pieceBelow]));
                    }
                }
            } 
        }
        // se devuelven las acciones generadas
        return resultActions;
    }



    public List<Action> GetPlan() {
        /*
        * Obtiene el plan para alcanzar el estado objetivo.
        *
        * Returns:
        *    List<Action>: Una lista de acciones que llevan al estado objetivo.
        */
        
        // hacemos una copia del mundo actual
        State currentState = new(CurrentState.allPredicates.ToList(), CurrentState.pastActions.ToList());
        // lo añadimos a la lista de estados pendientes de procesar
        List<State> statesLeftToProcess = [currentState];
        // inicializamos vacía la lista de visitados
        List<State> visitedStates = [];
        
        while (true) {
            // cogemos el primero
            currentState = statesLeftToProcess[0];
            // y lo quitamos
            statesLeftToProcess.RemoveAt(0);
            // y lo añadimos a visitadosa
            visitedStates.Add(currentState);
            
            // si estamos en la meta, paramos y devolvemos las acciones que hemos usado para llegar hasta ahí
            if (CheckGoal(currentState)) {
                Console.WriteLine();
                Console.WriteLine("Hemos encontrado una solución en " + visitedStates.Count + " nodos.");
                // sobreescribimos el mundo actual para devolver las acciones más tarde
                CurrentState = currentState;
                break;
            }

            // generamos todas las posibles acciones para el estado actual
            List<Action> possibleActions = GenerateActions(currentState);

            // aplicamos cada una de esas acciones y añadir el estado resultante
            foreach (Action currentAction in possibleActions) {
                // para cada postcondición TRUE, añadirla al estado actual
                // para cada postcondición FALSE, si está en TRUE se quita, y si no nada
                State newState = new(currentState.allPredicates.ToList(), currentState.pastActions.ToList());
                // esto es aplicar las postcondiciones
                foreach (Predicate p in currentAction.Postconditions) {
                    if (p.State == true) {
                        newState.allPredicates.Add(p);
                    }
                    else {
                        for (int i = newState.allPredicates.Count - 1; i >= 0; i--) {
                            Predicate p2 = newState.allPredicates[i];
                            if (p.Name == p2.Name && p.Members.SequenceEqual(p2.Members)) {                                
                                newState.allPredicates.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                // le añadimos al historial la acción que acabamos de ejecutar
                newState.pastActions.Add(currentAction);
                // añadimos este estado a la lista de estados pendientes solo si es "nuevo"
                if (!isContainedIn(newState, visitedStates) && !isContainedIn(newState, statesLeftToProcess)) {
                    statesLeftToProcess.Add(newState);
                }
            }
        }
        // devolvemos la lista de acciones ejecutadas hasta la meta
        return CurrentState.pastActions;
    } 
}

public class PlannerH : Planner {
    // la subclase PlannerH es casi idéntica a Planner,
    // excepto porque sobreescribe la generación de acciones para
    // el problema de Hanoi 
    public PlannerH(State inputCurrentState, List<State> inputGoal) : base(inputCurrentState, inputGoal) {}
        /*
     * Inicializa una nueva instancia de la clase PlannerH.
     *
     * Args:
     *    inputCurrentState (State): El estado actual del planificador.
     *    inputGoal (List<State>): Una lista de estados objetivo para el planificador.
     */
    
    
    
    public override List<Action> GenerateActions(State state) {
            /*
    * Genera las acciones posibles para un estado.
    *
    * Args:
    *    state (State): El estado para el que se van a generar las acciones.
    *
    * Returns:
    *    List<Action>: Una lista de acciones posibles para el estado.
    */
        
        // para cada bloque libre, moverlo a todos los demás bloques libres
        List<Action> resultActions = [];

        foreach (Predicate currentPredicate1 in state.allPredicates) {
            if (currentPredicate1.Name == "Free" && currentPredicate1.Members[0].Name != "Table" && !currentPredicate1.Members[0].Name.Contains("Rod")) {
                // extraemos la pieza actual
                Piece currentPiece = currentPredicate1.Members[0];
                Piece pieceBelow = null;
                // encontrar la pieza que está debajo
                foreach (Predicate currentPredicate2 in state.allPredicates) {
                    if (currentPredicate2.Name == "On" && currentPredicate2.Members[0] == currentPiece && currentPredicate2.Members[1].Name != "Table") {
                        pieceBelow = currentPredicate2.Members[1];
                        break;
                    }
                }

                if (pieceBelow != null) {
                    // moverlo a todos los demás bloques libres
                    foreach (Predicate currentPredicate3 in state.allPredicates) {
                        // si encontramos otra que está libre
                        if (currentPredicate3.Name == "Free" && currentPredicate3.Members[0].Name != "Table" && currentPredicate1 != currentPredicate3 && currentPiece.Value <= currentPredicate3.Members[0].Value) {
                            resultActions.Add(new MovePiece([currentPiece, pieceBelow, currentPredicate3.Members[0]]));
                        }
                    }
                }
            } 
        }
        // se devuelven las acciones generadas
        return resultActions;
    }
}