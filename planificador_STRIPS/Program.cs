class Program 
{
    static void Main(string[] args) {
        // la ejecución de cada uno de estos escenarios consiste en lo siguiente:
        //    1. Crear las piezas que se van a usar
        //    2. Crear los predicados para el estados inicial y las metas
        //    3. Crear un agente que incorpore toda esta información
        //    4. Obtener el plan (y mostrarlo)
        //    5. Ejecutar el plan (y mostrarlo)
        
        // Problema 1 Escenario 1
        // executeBlockWorld1_1();

        // Problema 1 Escenario 2
        // executeBlockWorld1_2();

        // Problema 2
        // EL VALOR QUE SE LE PASA A executeHanoi ES EL NÚMERO DE DISCOS DESEADO
        executeHanoi(1);

    }



    // Ejecuta el escenario 1 del Mundo de Bloques.
    static void executeBlockWorld1_1() {
        Console.WriteLine();
        Console.WriteLine("Ejecutando el escenario 1 del Mundo de Bloques...");

        Piece pieceA = new("A");
        Piece pieceB = new("B");
        Piece pieceC = new("C");
        Piece table = new("Table");

        List<Piece> all_pieces = [pieceA, pieceB, pieceC];

        State initial_state = new ([new Predicate("On", [pieceA, table], true),
                                    new Predicate("On", [pieceB, table], true),
                                    new Predicate("On", [pieceC, table], true),

                                    new Predicate("Free", [pieceA], true),
                                    new Predicate("Free", [pieceB], true),
                                    new Predicate("Free", [pieceC], true),
                                    new Predicate("Free", [table], true)],
                                    []);
        
        State goal_state =  new ([new Predicate("On", [pieceA, pieceB], true),
                                  new Predicate("On", [pieceB, pieceC], true),
                                  new Predicate("On", [pieceC, table], true),

                                  new Predicate("Free", [pieceA], true)
                                  ], 
                                  []);

        Agent agent = new (initial_state, [goal_state], 0);

        agent.getSolution();

        agent.executePlan();

    }



    // Ejecuta el escenario 2 del Mundo de Bloques.
    static void executeBlockWorld1_2() {
        Console.WriteLine();
        Console.WriteLine("Ejecutando el escenario 2 del Mundo de Bloques...");

        Piece pieceA = new("A");
        Piece pieceB = new("B");
        Piece pieceC = new("C");
        Piece pieceD = new("D");
        Piece pieceE = new("E");
        Piece pieceF = new("F");
        Piece table = new("Table");

        List<Piece> all_pieces = [pieceA, pieceB, pieceC, pieceD, pieceE, pieceF];

        State initial_state = new ([new Predicate("On", [pieceA, table], true),
                                    new Predicate("On", [pieceE, table], true),
                                    new Predicate("On", [pieceD, table], true),

                                    new Predicate("On", [pieceB, pieceA], true),
                                    new Predicate("On", [pieceF, pieceE], true),

                                    new Predicate("On", [pieceC, pieceB], true),

                                    new Predicate("Free", [pieceC], true),
                                    new Predicate("Free", [pieceF], true),
                                    new Predicate("Free", [pieceD], true),
                                    new Predicate("Free", [table], true)],
                                    []);
        
        State goal_state =  new ([new Predicate("On", [pieceC, table], true),
                                  new Predicate("On", [pieceB, table], true),
                                  new Predicate("On", [pieceA, table], true),

                                  new Predicate("On", [pieceE, pieceC], true),
                                  new Predicate("On", [pieceF, pieceB], true),
                                  new Predicate("On", [pieceD, pieceA], true),

                                  new Predicate("Free", [table], true)
                                  ], 
                                  []);

        Agent agent = new (initial_state, [goal_state], 0);

        agent.getSolution();

        agent.executePlan();

    }



    // Ejecuta el problema de las Torres de Hanoi.
    static void executeHanoi(int n_of_discs = 3) {
        /*
        * Ejecuta el problema de las Torres de Hanoi.
        *
        * Args:
        *    n_of_discs (int): El número de discos deseado. Por defecto es 3.
        */
        Console.WriteLine();
        Console.WriteLine("Ejecutando problema de las Torres de Hanoi...");

        List<Piece> all_pieces = [];
        Piece table = new("Table");
        Piece rod1 = new("Rod1", 100000);
        Piece rod2 = new("Rod2", 100000);
        Piece rod3 = new("Rod3", 100000);

        List<Piece> all_rods = [rod1, rod2, rod3];
        
        int N_OF_DISCS = n_of_discs;

        for (int i = 1; i <= N_OF_DISCS; i++) {
            string name = string.Concat(i.ToString());
            all_pieces.Add(new Piece(name, i));
        }

        List<State> allRodStates = [];

        foreach (Piece rod in all_rods) {
            List<Predicate> futurePredicates = [new Predicate("On", [rod1, table], true),
                                                new Predicate("On", [rod2, table], true),
                                                new Predicate("On", [rod3, table], true)];
            List<Piece> pieces = all_pieces.ToList();
            pieces.Reverse();

            int i = pieces.Count - 1;
            while (i > 0) {
                futurePredicates.Add(new Predicate("On", [pieces[i], pieces[i-1]], true));
                i--;
            }
            
            futurePredicates.Add(new Predicate("On", [pieces[0], rod], true));
            allRodStates.Add(new State(futurePredicates, []));
        }

        State initial_state = allRodStates[0];
        initial_state.allPredicates.Add(new Predicate("Free", [all_pieces[0]], true));
        initial_state.allPredicates.Add(new Predicate("Free", [rod2], true));
        initial_state.allPredicates.Add(new Predicate("Free", [rod3], true));
        List<State> goal_states = [allRodStates[1], allRodStates[2]];

        Agent agent = new (initial_state, goal_states, 1);

        agent.getSolution();

        agent.executePlan();

    }

}