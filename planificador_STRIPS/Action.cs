public abstract class Action 
{
    // Obtiene o establece los prerrequisitos de la acción.
    public Predicate[] Prerequisites { get; set; } = [];
    // Obtiene o establece las postcondiciones de la acción.
    public Predicate[] Postconditions { get; set; } = [];
    // Obtiene o establece los participantes de la acción.
    public Piece[] Participants { get; set; } = [];
    // Inicializa una nueva instancia de la clase Action.
    public Action() {}
    
}

public class MovePiece : Action {
    public MovePiece(Piece[] inputParticipants) {
        /*
        * Inicializa una nueva instancia de la clase MovePiece.
        *
        * Args:
        *    inputParticipants (Piece[]): Los participantes de la acción.
        */
        Participants = inputParticipants;
        // la que se mueve
        Piece pX = Participants[0];
        // donde estaba X
        Piece pY = Participants[1];
        // a donde va X
        Piece pZ = Participants[2]; 
        
        Prerequisites = [new Predicate("Free", [pX], true),
                            new Predicate("Free", [pZ], true)];
        
        Postconditions = [new Predicate("On", [pX, pZ], true),
                            new Predicate("On", [pX, pY], false),
                            new Predicate("Free", [pY], true),
                            new Predicate("Free", [pZ], false)];
    }
}


public class MovePieceToTable : Action {
    public MovePieceToTable(Piece[] inputParticipants) {
        /*
        * Inicializa una nueva instancia de la clase MovePieceToTable.
        *
        * Args:
        *    inputParticipants (Piece[]): Los participantes de la acción.
        */
        Participants = inputParticipants;
        // la pieza que está debajo
        Piece pB = Participants[0];
        // la pieza que se mueve
        Piece pX = Participants[1];

        Prerequisites = [new Predicate("On", [pB, pX], true),
                            new Predicate("Free", [pB], true)];
        
        Postconditions = [new Predicate("On", [pB, new("Table")], true),
                            new Predicate("Free", [pX], true),
                            new Predicate("On", [pB, pX], false)];
    }
}