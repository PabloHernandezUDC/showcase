public class Agent 
{
    // Obtiene o establece el estado inicial del agente.
    public State InitialState { get; set; }
    // Obtiene o establece el estado actual del agente.
    public State CurrentState { get; set; }
    // Obtiene o establece los estados objetivo del agente.
    public List<State> GoalStates { get; set; }
    // Obtiene o establece el tipo de planificador del agente.
    public int PlannerType { get; set; }
    // 1                    -> Planificador de Hanoi
    // cualquier otro valor -> Planificador de Mundo de Bloques
    public List<Action> Plan { get; set; }


    public Agent(State inputInitialState, List<State> inputGoalStates, int inputPlannerType) {
        /*
        * Inicializa una nueva instancia de la clase Agent.
        *
        * Args:
        *    inputInitialState (State): El estado inicial del agente.
        *    inputGoalStates (List<State>): Una lista de estados objetivo para el agente.
        *    inputPlannerType (int): El tipo de planificador utilizado por el agente.
        */

        InitialState = inputInitialState;
        // el estado del agente (el que irá cambiando) empieza siendo
        // el mismo que el inicial
        CurrentState = inputInitialState;
        GoalStates = inputGoalStates;
        PlannerType = inputPlannerType;
    }


    public void showState(State s) {
        /*
        * Muestra el estado actual del agente.
        *
        * Args:
        *    s (State): El estado que se va a mostrar.
        *
        * Este método toma el estado actual del agente y lo muestra en la consola.
        * Primero, se eliminan los predicados "Free" para que no interfieran.
        * Luego, se busca y se muestra cada pieza que está sobre la mesa, y todas las piezas que están encima de ella.
        * Finalmente, se muestra la mesa en sí.
        */
        
        // inicializamos el escenario vacío
        List<List<string>> scenario = [];
        // y guardamos los predicados de s
        List<Predicate> predicates = s.allPredicates.ToList();

        // mientras haya predicados de On
        // para cada Predicado de encima de la mesa, buscar las q están encima
        // para cada uno de esos, los q estén encima y etc

        // quitamos los Frees para que no estorben
        predicates.RemoveAll(x => x.Name == "Free");

        // encontramos los que están encima de la mesa y los quitamos
        List<Predicate> firstLayerPredicates = predicates.FindAll(x => x.Members[1].Name == "Table");
        predicates.RemoveAll(x => x.Name == "On" && x.Members[1].Name == "Table");

        foreach (Predicate pieceOnTable in firstLayerPredicates) {
            // encontramos la pieza X que está encima de la pieza Y que está
            // encima de la mesa, y así indefinidamente hasta que llegamos a la
            // "cima" de la pila de piezas
            List<string> currentColumn = [pieceOnTable.Members[0].Name];
            Predicate previousPiecePredicate = pieceOnTable;

            while (true) {
                Predicate nextPredicate = predicates.Find(x => x.Members[1].Name == previousPiecePredicate.Members[0].Name);

                if (nextPredicate == null) {
                    break;
                }
                else {
                    predicates.Remove(nextPredicate);
                    currentColumn.Add(nextPredicate.Members[0].Name);
                    previousPiecePredicate = nextPredicate;
                }
            }
            // cuando hemos terminado con una columna, se añade a scenario
            scenario.Add(currentColumn);
        }

        // tenemos que trasponer la matriz porque sino la "mesa" estaría a la 
        // izquierda en vez de abajo

        // convertimos la lista de listas en una matriz bidimensional
        string[,] matrix = new string[scenario.Count, scenario.Max(row => row.Count)];
        for (int i = 0; i < scenario.Count; i++)
        {
            for (int j = 0; j < scenario[i].Count; j++)
            {
                 // valor por defecto para los nulls
                matrix[i, j] = scenario[i][j] ?? "";
            }
        }
        // guardamos el tamaño máximo de los elementos que nos vamos encontrando
        int maxLength = 0;

        // trasponemos la matriz
        string[,] transposedMatrix = new string[matrix.GetLength(1), matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                // valor por defecto para los nulls
                transposedMatrix[j, i] = matrix[i, j] ?? "";
                if (transposedMatrix[j, i].Length > maxLength) {
                    // si el actual es mayor que el máximo, se sobreescribe
                    maxLength = transposedMatrix[j, i].Length;
                }
            }
        }
        // le sumamos espacio a la longitud máxima para que quede todo bien separado
        maxLength += 3; 
        // mostramos la matriz traspuesta
        for (int i = transposedMatrix.GetLength(0) - 1; i >= 0; i--) {
            for (int j = 0; j < transposedMatrix.GetLength(1); j++) {
                // Agregamos espacios adicionales entre las columnas
                Console.Write(""); 

                if (transposedMatrix[i, j] != null) {
                    string elem = transposedMatrix[i, j];
                    // si está vacío, será una varilla 
                    if (elem == "" && PlannerType == 1) { 
                        elem = "|";
                    }
                    // le sumamos espacios por la derecha hasta que llegue a la longitud deseada
                    while (elem.Length < maxLength) { 
                        elem += " ";
                    }
                    Console.Write(elem);
                }
            }            
            Console.WriteLine();
        }
        // imprimimos la mesa en base al tamaño correcto
        Console.WriteLine(new string('—', (maxLength-1) * transposedMatrix.GetLength(1)));
    }


    public void getSolution() {
        /*
        * Obtiene la solución al problema planteado para el agente.
        *
        * Este método crea un nuevo planificador con el estado inicial y los estados objetivo del agente.
        * Si el tipo de planificador es 1, se crea un planificador con heurística.
        * Luego, se obtiene el plan del planificador y se muestra en la consola.
        */
        
        // cogemos el tipo de planificador, y si es 1, se usa el de Hanoi
        Planner planner = new Planner(InitialState, GoalStates);
        if (PlannerType == 1) {
            planner = new PlannerH(InitialState, GoalStates);
        }
        
        // se obtiene el plan y se guarda
        Plan = planner.GetPlan();

        // mostramos por pantalla todas las acciones por orden
        Console.WriteLine();
        Console.WriteLine("Este es el plan:");
        foreach (Action a in Plan) {
            Console.Write(a.GetType() + " ");
            foreach (Piece pi in a.Participants) {
                Console.Write(pi.Name + " ");
            }
            Console.WriteLine();
        }
    }


    public void executePlan() {
        /*
        * Ejecuta el plan obtenido para el agente.
        *
        * Este método muestra el estado actual del mundo antes de aplicar las acciones.
        * Luego, para cada acción en el plan, se aplican las postcondiciones de la acción al estado actual.
        * Si la postcondición es verdadera, se añade al estado actual.
        * Si la postcondición es falsa, se busca y se elimina del estado actual.
        * Finalmente, se muestra el estado después de aplicar cada acción.
        */        
        Console.WriteLine();
        Console.WriteLine("El mundo antes de aplicar las acciones:");
        showState(CurrentState);

        // aplicamos las acciones igual que lo hacíamos en el planificador
        foreach (Action currentAction in Plan) {
            foreach (Predicate p in currentAction.Postconditions) {
                if (p.State == true) {
                    CurrentState.allPredicates.Add(p);
                }
                else {
                    for (int i = CurrentState.allPredicates.Count - 1; i >= 0; i--) {
                        Predicate p2 = CurrentState.allPredicates[i];
                        if (p.Name == p2.Name && p.Members.SequenceEqual(p2.Members)) {                                
                            CurrentState.allPredicates.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Siguiente acción:");
            showState(CurrentState);
        }
    }
}