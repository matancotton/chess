using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ChessGameLauncher
    {
        static void Main(string[] args)
        {
            new ChessGame().Play();
        }
    }
    public class ChessGame
    {
        public ChessPiece[,] ChessBoard { get; set; }
        List<ChessPiece[,]> boardCopysContainer = new List<ChessPiece[,]>();
        bool isWhite =true;
        int movesCounter;
        public void Play()
        {
            bool isGameOver = false;
            string gameResult = "";
            createBoard();
            printBoard();
            boardCopysContainer.Add(copyBoard());
            do
            {
                cancelPawnSkip();
                playerGo();
                printBoard();
                if (reNewPiece())
                    printBoard();
                if (IsInCheck(!isWhite))
                {
                    if (!isKingAbleToMove(isWhite?"black":"white"))
                    {
                        gameResult = string.Format("{0} player lost the game...",isWhite?"Black":"White");
                        isGameOver = true;
                        continue;
                    }
                    Console.WriteLine("{0} player is in check", isWhite ? "Black" : "White");
                }
                else if (!isKingAbleToMove(isWhite ? "black" : "white") || (notEnoughtPieces(isWhite) && notEnoughtPieces(!isWhite)) || movesCounter == 50 || isThreeFoldRepetition())
                {
                    if (movesCounter == 50)
                        gameResult += "you played 50 Moves\n";
                    gameResult += "the game ended in a tie";
                    isGameOver = true;
                    continue;
                }
                isWhite = !isWhite;
            } while (!isGameOver);
            Console.WriteLine(gameResult);
        }
        void createBoard()
        {
            ChessBoard =new ChessPiece[,] { {new Rook(false,new Location{Row=0,Column=0 }),new Knight(false,new Location{Row=0,Column=1 }),new Bishoph(false,new Location{Row=0,Column=2 }),new Queen(false,new Location{Row=0,Column=3 }),new King(false,new Location{Row=0,Column=4 }),new Bishoph(false,new Location{Row=0,Column=5 }),new Knight(false,new Location{Row=0,Column=6 }),new Rook(false,new Location{Row=0,Column=7 }) },
                                            {new Pawn(false,new Location{Row=1,Column=0 }),new Pawn(false,new Location{Row=1,Column=1 }),new Pawn(false,new Location{Row=1,Column=2 }),new Pawn(false,new Location{Row=1,Column=3 }),new Pawn(false,new Location{Row=1,Column=4 }),new Pawn(false,new Location{Row=1,Column=5 }),new Pawn(false,new Location{Row=1,Column=6 }),new Pawn(false,new Location{Row=1,Column=7 }) },
                                            {null,null,null,null,null,null,null,null },
                                            {null,null,null,null,null,null,null,null },
                                            {null,null,null,null,null,null,null,null },
                                            {null,null,null,null,null,null,null,null },
                                            {new Pawn(true,new Location{Row=6,Column=0 }),new Pawn(true,new Location{Row=6,Column=1 }),new Pawn(true,new Location{Row=6,Column=2 }),new Pawn(true,new Location{Row=6,Column=3 }),new Pawn(true,new Location{Row=6,Column=4 }),new Pawn(true,new Location{Row=6,Column=5 }),new Pawn(true,new Location{Row=6,Column=6 }),new Pawn(true,new Location{Row=6,Column=7 }) },
                                            {new Rook(true,new Location{Row=7,Column=0 }),new Knight(true,new Location{Row=7,Column=1 }),new Bishoph(true,new Location{Row=7,Column=2}),new Queen(true,new Location{Row=7,Column=3 }),new King(true,new Location{Row=7,Column=4 }),new Bishoph(true,new Location{Row=7,Column=5 }),new Knight(true,new Location{Row=7,Column=6 }),new Rook(true,new Location{Row=7,Column=7 }) } };
        }
        void printBoard()
        {
            Console.Write("  ");
            for (int i = 65; i < 73; i++)
                Console.Write((char)i + "  ");
            Console.WriteLine();
            for (int i = 0; i < ChessBoard.GetLength(0); i++)
            {
                Console.Write(i + 1 + " ");
                for (int j = 0; j < ChessBoard.GetLength(1); j++)
                {
                    if (ChessBoard[i, j] is null)
                        Console.Write("EE ");
                    else
                        Console.Write(ChessBoard[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        bool isThreeFoldRepetition()
        {
            int count = 0;
            for (int i = 0; i < boardCopysContainer.Count; i++)
            {
                if (boardCopysContainer[i] != null)
                {
                    if (areBoardsEquals(boardCopysContainer[i]))
                        count++;
                }
            }
            if (count == 3)
                return true;
            return false;
        }
        bool areBoardsEquals(ChessPiece[,] boardCopy)
        {
            for (int x = 0; x < ChessBoard.GetLength(0); x++)
                for (int y = 0; y < ChessBoard.GetLength(1); y++)
                    if (ChessBoard[x, y] != null)
                    {
                        if (!ChessBoard[x, y].Equals(boardCopy[x, y]))
                            return false;
                    }
                    else
                        if (boardCopy[x, y] != null)
                            return false;
            return true;
        }
        void playerGo()
        {
            while (!turn()) ;
        }
        void createMove(out Location startLocation, out Location endLocation)
        {
            bool isValidInput = false;
            int startRow = 0;
            int startColumn = 0;
            int endRow = 0;
            int endColumn = 0;
            Console.WriteLine("please enter {0} turn:\n", isWhite ? "White" : "Black");
            string input = Console.ReadLine();
            while (!isValidInput)
            {
                input = input.Trim();
                if (input.Length == 4)
                {
                    input = input.ToUpper();
                    startRow = findNumber(input[1]);
                    startColumn = findLetter(input[0]);
                    endRow = findNumber(input[3]);
                    endColumn = findLetter(input[2]);
                    if (startColumn != -1 && endColumn != -1)
                        if (startRow != -1 && endRow != -1)
                        {
                            isValidInput = true;
                            continue;
                        }
                }
                Console.WriteLine("\nyou have enterd an invalid input...\n");
                Console.WriteLine("please enter {0} turn:\n", isWhite ? "White" : "Black");
                input = Console.ReadLine();
            }
            startLocation = new Location { Row = startRow, Column = startColumn };
            endLocation = new Location { Row = endRow, Column = endColumn };
        }
        
        bool turn()
        {
            ChessPiece [,] temporaryBoard = copyBoard();
            Location startPosition;
            Location endPosition;
            createMove(out startPosition,out endPosition);
            ChessPiece piece = ChessBoard[startPosition.Row, startPosition.Column];
            ChessPiece temporaryPiece = temporaryBoard[startPosition.Row, startPosition.Column];
            if (piece != null)
            {
                if (piece.IsWhite == isWhite)
                {
                    if (piece.IsValidMove(this, endPosition))
                    {
                        move(startPosition, endPosition);
                        if (IsInCheck(isWhite))
                        {
                            Console.WriteLine("\nIllegal Move for {0} player\n", isWhite ? "White" : "Black");
                            ChessBoard = temporaryBoard;
                            return false;
                        }
                        if (temporaryPiece.IsEating(this, endPosition) || temporaryPiece is Pawn)
                        {
                            boardCopysContainer.Clear();
                            movesCounter = 0;
                        }
                        else
                        {
                            boardCopysContainer.Add(copyBoard());
                            movesCounter++;
                        }
                        return true;
                    }
                }
            }
            Console.WriteLine("\nIllegal Move for {0} player\n", isWhite ? "White" : "Black");
            return false;
        }
        int findNumber(char ch)
        {
            string numbers = "12345678";
            for (int i = 0; i < numbers.Length; i++)
                if (ch == numbers[i])
                    return i;
            return -1;
        }
        int findLetter(char ch)
        {
            string letters = "ABCDEFGH";
            for (int i = 0; i < letters.Length; i++)
                if (ch == letters[i])
                    return i;
            return -1;
        }
        public bool IsInCheck(bool isWhite)
        {
            King king = null;
            for (int i = 0; i < ChessBoard.GetLength(0) && king == null; i++)
                for (int j = 0; j < ChessBoard.GetLength(1) && king == null; j++)
                    if (ChessBoard[i, j] is King)
                        if (ChessBoard[i, j].IsWhite == isWhite)
                            king = (King)ChessBoard[i, j];
            for (int i = 0; i < ChessBoard.GetLength(0); i++)
                for (int j = 0; j < ChessBoard.GetLength(1); j++)
                    if (ChessBoard[i, j] != null && !(ChessBoard[i,j] is King))
                        if (ChessBoard[i, j].IsWhite!=isWhite)
                            if (ChessBoard[i, j].IsValidMove(this, king.ChessPieceLocation))
                                return true;
            return false;
        }
        bool isKingAbleToMove(string player)
        {
            bool isWhitePlayer = player == "white" ? true : false;
            bool kingCanMove = false;
            Location from;
            Location to;
            ChessPiece[,] temporaryBoard;
            for (int i = 0; i < ChessBoard.GetLength(0); i++)
                for (int j = 0; j < ChessBoard.GetLength(1); j++)
                    if (ChessBoard[i, j] != null && ChessBoard[i, j].IsWhite == isWhitePlayer)
                    {
                        from = new Location { Row = i, Column = j };
                        for (int row = 0; row < ChessBoard.GetLength(0); row++)
                            for (int col = 0; col < ChessBoard.GetLength(1); col++)
                            {
                                to = new Location { Row = row, Column = col };
                                if (ChessBoard[from.Row, from.Column].IsValidMove(this, to))
                                {
                                    temporaryBoard = copyBoard();
                                    move(from, to);
                                    if (!IsInCheck(isWhitePlayer))
                                        kingCanMove = true;
                                    ChessBoard = temporaryBoard;
                                }
                                if (kingCanMove)
                                    return true;
                            }
                    }
            return false;
        }
        void cancelPawnSkip()
        {
            for (int i = 0; i < ChessBoard.GetLength(0); i++)
                for (int j = 0; j < ChessBoard.GetLength(1); j++)
                    if (ChessBoard[i, j] is Pawn && ChessBoard[i, j].IsWhite == isWhite)
                        if (((Pawn)ChessBoard[i, j]).skip)
                            ((Pawn)ChessBoard[i, j]).skip = false;
        }
        bool notEnoughtPieces(bool isWhite)
        {
            int bishophAndKnightCounter = 0;
            for (int i = 0; i < ChessBoard.GetLength(0); i++)
                for (int j = 0; j < ChessBoard.GetLength(1); j++)
                    if (ChessBoard[i, j] != null)
                        if (ChessBoard[i, j].IsWhite==isWhite)
                        {
                            switch (ChessBoard[i, j].Name[1])
                            {
                                case 'P':
                                case 'R':
                                case 'Q':
                                    return false;
                                case 'N':
                                    bishophAndKnightCounter++;
                                    break;
                                case 'B':
                                    bishophAndKnightCounter++;
                                    break;
                            }
                        }
            if (bishophAndKnightCounter >= 2)
                return false;
            return true;
        }
        bool reNewPiece()
        {
            int row = isWhite ? 0 : 7;
            string ch = "" + (isWhite ? 'W' : 'B');
            bool reNew = false;
            for (int i = 0; i < 8; i++)
                if (ChessBoard[row, i] is Pawn)
                    if (ChessBoard[row, i].IsWhite == isWhite)
                    {
                        do
                        {
                            Console.WriteLine("please type full Name of a piece you yould like to add to the board");
                            string input = Console.ReadLine();
                            input = input.Trim();
                            input = input.ToUpper();
                            Location location = new Location { Row = row, Column = i };
                            switch (input)
                            {
                                case "QUEEN":
                                    ChessBoard[row, i] = new Queen(isWhite, location); return true;
                                case "ROOK":
                                    ChessBoard[row, i] = new Rook(isWhite, location); return true;
                                case "BISHOPH":
                                    ChessBoard[row, i] = new Bishoph(isWhite, location); return true;
                                case "KNIGHT":
                                    ChessBoard[row, i] = new Knight(isWhite, location); return true;
                            }
                            Console.WriteLine("you enterd an invalid piece");
                        } while (!reNew);
                    }
            return false;
        }
        void move(Location start, Location end)
        {
            ChessPiece piece = ChessBoard[start.Row, start.Column];
            if (piece is Pawn)
            {
                Pawn pawn = piece as Pawn;
                int eatRow = pawn.IsWhite ? -1 : 1;
                if (pawn.IsEnPassant(this,end,eatRow))
                {
                    ChessBoard[start.Row, start.Column] = null;
                    ChessBoard[end.Row, end.Column] = piece;
                    ChessBoard[end.Row, end.Column].ChessPieceLocation = end;
                    ChessBoard[end.Row-eatRow, end.Column] = null;
                    movesCounter = 0;
                    return;
                }
                if (start.Row == end.Row + 2 || start.Row == end.Row - 2)
                    pawn.skip = true;
            }
            if (piece is King)
            {
                King king = piece as King;
                if (king.IsAbleCastling(this,end))
                {
                    int castlingRow = king.IsWhite ? 7 : 0;
                    if (king.smallCastling(this,end))
                        move(new Location { Row = castlingRow, Column = 7 }, new Location { Row = castlingRow, Column = 5 });
                    else
                        move(new Location { Row = castlingRow, Column = 0 }, new Location { Row = castlingRow, Column = 3 });
                }
                king.AbleCastling = false;
            }
            if (piece is Rook)
                ((Rook)piece).AbleCastling = false;
            ChessBoard[start.Row, start.Column] = null;
            ChessBoard[end.Row,end.Column] = piece;
            ChessBoard[end.Row, end.Column].ChessPieceLocation = end;
        }
        ChessPiece[,] copyBoard()
        {
            ChessPiece[,] chessCopy = new ChessPiece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ChessBoard[i, j] == null)
                    {
                        chessCopy[i, j] = null;
                        continue;
                    }
                    switch (ChessBoard[i, j].ToString()[1])
                    {
                        case 'P':
                            chessCopy[i, j] = new Pawn(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            ((Pawn)chessCopy[i, j]).skip = ((Pawn)ChessBoard[i, j]).skip;
                            break;
                        case 'R':
                            chessCopy[i, j] = new Rook(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            ((Rook)chessCopy[i, j]).AbleCastling = ((Rook)ChessBoard[i, j]).AbleCastling;
                            break;
                        case 'K':
                            chessCopy[i, j] = new King(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            ((King)chessCopy[i, j]).AbleCastling = ((King)ChessBoard[i, j]).AbleCastling;
                            break;
                        case 'B':
                            chessCopy[i, j] = new Bishoph(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            break;
                        case 'N':
                            chessCopy[i, j] = new Knight(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            break;
                        case 'Q':
                            chessCopy[i, j] = new Queen(ChessBoard[i, j].IsWhite, ChessBoard[i, j].ChessPieceLocation);
                            break;
                    }
                }
            }
            return chessCopy;
        }
    }
    public class ChessPiece
    {
        public string Name { get; protected set; }
        public bool IsWhite { get; set; }
        public Location ChessPieceLocation { get; set; }
        public ChessPiece(bool isWhite,Location location)
        {
            IsWhite = isWhite;
            Name = IsWhite?"W":"B";
            ChessPieceLocation = location;
        }
        public override string ToString()
        {
            return Name;
        }
        public virtual bool IsValidMove(ChessGame board,Location location)
        {
            if (ChessPieceLocation==location)
                return false;
            if (board.ChessBoard[location.Row, location.Column] != null)
            {
                if (IsEating(board, location))
                    return true;
                return false;
            }
            return true;
        }
        public virtual bool IsEating(ChessGame board,Location location)
        {
            if (board.ChessBoard[location.Row, location.Column] != null)
                if(IsWhite != board.ChessBoard[location.Row, location.Column].IsWhite)
                    return true;
            return false;
        }
        protected virtual bool HasAvailablePath(ChessGame board,Location location)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            ChessPiece other = obj as ChessPiece;
        return Name == other.Name && IsWhite == other.IsWhite && ChessPieceLocation == other.ChessPieceLocation;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ IsWhite.GetHashCode() ^ ChessPieceLocation.GetHashCode();
        }
    }
    class Pawn : ChessPiece
    {
        public bool skip;
        public Pawn(bool isWhite,Location location) : base(isWhite,location)
        { Name += "P"; }

        protected override bool HasAvailablePath(ChessGame board, Location location)
        {
            if (board.ChessBoard[location.Row, location.Column] != null)
                return false;
            if (IsWhite)
            {
                if (board.ChessBoard[ChessPieceLocation.Row - 1, ChessPieceLocation.Column] != null)
                    return false;
            }
            else
                if (board.ChessBoard[ChessPieceLocation.Row + 1, ChessPieceLocation.Column] != null)
                return false;
            return true;
        }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (!base.IsValidMove(board, location))
                return false;
            if (ChessPieceLocation.Column == location.Column)
            {
                if (IsWhite)
                {
                    if (ChessPieceLocation.Row == 6 && location.Row == 4)
                        return HasAvailablePath(board, location);
                    if (ChessPieceLocation.Row-1 == location.Row)
                        return HasAvailablePath(board, location);
                }
                else
                {
                    if (ChessPieceLocation.Row == 1 && location.Row == 3)
                        return HasAvailablePath(board, location);
                    if (location.Row == ChessPieceLocation.Row + 1)
                        return HasAvailablePath(board, location);
                }
            }
            else
                return IsEating(board, location);
            return false;
        }
        public override bool IsEating(ChessGame board,Location location)
        {
            int eatRow = IsWhite ? -1 : 1;
            if ((location.Row != (ChessPieceLocation.Row + eatRow)) || (location.Column != ChessPieceLocation.Column - 1 && location.Column != ChessPieceLocation.Column + 1))
                return false;
            if (board.ChessBoard[location.Row, location.Column] == null)
                return IsEnPassant(board, location, eatRow);
            return base.IsEating(board, location);
        }
        public bool IsEnPassant(ChessGame board,Location location, int eatRow)
        {
            if (board.ChessBoard[location.Row - eatRow, location.Column] != null && board.ChessBoard[location.Row - eatRow, location.Column] is Pawn)
                if (board.ChessBoard[location.Row - eatRow, location.Column].IsWhite != IsWhite && ((Pawn)board.ChessBoard[location.Row - eatRow, location.Column]).skip)
                    return true;
            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Pawn other = obj as Pawn;
            if (other == null)
                return false;
            return base.Equals(obj)&&skip==other.skip;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ skip.GetHashCode();
        }
    }
    class Rook : ChessPiece
    {
        public bool AbleCastling = true;
        public Rook(bool isWhite, Location location) : base(isWhite, location) { Name += "R"; }

        protected override bool HasAvailablePath(ChessGame board,Location location)
        {
            int start, end;
            if (ChessPieceLocation.Column == location.Column)
            {
                start = ChessPieceLocation.Row < location.Row ? ChessPieceLocation.Row : location.Row;
                end = ChessPieceLocation.Row > location.Row ? ChessPieceLocation.Row : location.Row;
                for (int i=start+1; i<end;i++)
                    if (board.ChessBoard[i,location.Column]!=null)
                        return false;
                return true;
            }
            else
            {
                start = ChessPieceLocation.Column < location.Column ? ChessPieceLocation.Column : location.Column;
                end = ChessPieceLocation.Column > location.Column ? ChessPieceLocation.Column : location.Column;
                for (int i = start+1; i < end; i++)
                    if (board.ChessBoard[location.Row, i] != null)
                        return false;
                return true;
            }
        }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (!base.IsValidMove(board, location))
                return false;
            if (ChessPieceLocation.Row != location.Row && ChessPieceLocation.Column!=location.Column)
                return false;
            if (HasAvailablePath(board, location))
                return true;
            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Rook other = obj as Rook;
            if (other == null)
                return false;
            return base.Equals(obj)&&AbleCastling==other.AbleCastling;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()^AbleCastling.GetHashCode();
        }
    }
    class Bishoph : ChessPiece
    {
        public Bishoph(bool isWhite, Location location) : base(isWhite, location) { Name += "B"; }

        protected override bool HasAvailablePath(ChessGame board, Location location)
        {
            int row = ChessPieceLocation.Row - 1;
            int column = ChessPieceLocation.Column - 1;
            while (row > location.Row && column > location.Column)
            {
                if (board.ChessBoard[row, column] != null)
                    return false;
                row--;
                column--;
            }
            row = ChessPieceLocation.Row - 1;
            column = ChessPieceLocation.Column + 1;
            while (row > location.Row && column < location.Column)
            {
                if (board.ChessBoard[row, column] != null)
                    return false;
                row--;
                column++;
            }
            row = ChessPieceLocation.Row + 1;
            column = ChessPieceLocation.Column - 1;
            while (row < location.Row && column > location.Column)
            {
                if (board.ChessBoard[row, column] != null)
                    return false;
                row++;
                column--;
            }
            row = ChessPieceLocation.Row + 1;
            column = ChessPieceLocation.Column + 1;
            while (row < location.Row && column < location.Column)
            {
                if (board.ChessBoard[row, column] != null)
                    return false;
                row++;
                column++;
            }
            return true;
        }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (!base.IsValidMove(board, location))
                return false;
            int row = ChessPieceLocation.Row-1;
            int column = ChessPieceLocation.Column-1;
            while (row>=0 && column >=0)
            {
                if (location.Row == row && location.Column == column)
                    return HasAvailablePath(board, location);
                row--;
                column--;
            }
            row = ChessPieceLocation.Row - 1;
            column = ChessPieceLocation.Column + 1;
            while (row >= 0 && column < board.ChessBoard.GetLength(1))
            {
                if (location.Row == row && location.Column == column)
                    return HasAvailablePath(board, location);
                row--;
                column++;
            }
            row = ChessPieceLocation.Row + 1;
            column = ChessPieceLocation.Column - 1;
            while (row < board.ChessBoard.GetLength(0) && column >= 0)
            {
                if (location.Row == row && location.Column == column)
                    return HasAvailablePath(board, location);
                row++;
                column--;
            }
            row = ChessPieceLocation.Row + 1;
            column = ChessPieceLocation.Column + 1;
            while (row < board.ChessBoard.GetLength(0) && column < board.ChessBoard.GetLength(1))
            {
                if (location.Row == row && location.Column == column)
                    return HasAvailablePath(board, location);
                row++;
                column++;
            }
            return false;
        }
    }
    class Knight : ChessPiece
    {
        public Knight(bool isWhite, Location location) : base(isWhite, location) { Name += "N"; }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (ChessPieceLocation.Row == location.Row+2)
                if (ChessPieceLocation.Column == location.Column + 1 || ChessPieceLocation.Column == location.Column - 1)
                    return true;
            if (ChessPieceLocation.Row == location.Row - 2)
                if (ChessPieceLocation.Column == location.Column + 1 || ChessPieceLocation.Column == location.Column - 1)
                    return true;
            if (ChessPieceLocation.Column == location.Column + 2)
                if (ChessPieceLocation.Row == location.Row + 1 || ChessPieceLocation.Row == location.Row - 1)
                    return true;
            if (ChessPieceLocation.Column == location.Column - 2)
                if (ChessPieceLocation.Row == location.Row + 1 || ChessPieceLocation.Row == location.Row - 1)
                    return true;
            return false;
        }
    }
    class Queen : ChessPiece
    {
        public Queen(bool isWhite, Location location) : base(isWhite, location) { Name += "Q"; }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (!base.IsValidMove(board, location))
                return false;
            Rook rook = new Rook(IsWhite, ChessPieceLocation);
            Bishoph bishoph = new Bishoph(IsWhite, ChessPieceLocation);
            if (rook.IsValidMove(board,location) || bishoph.IsValidMove(board, location))
                return true;
            return false;
        }
    }
    class King : ChessPiece
    {
        public bool AbleCastling = true;
        public King(bool isWhite, Location location) : base(isWhite, location) { Name += "K"; }
        public override bool IsValidMove(ChessGame board, Location location)
        {
            if (!base.IsValidMove(board, location))
                return false;
            if (IsAbleCastling(board, location))
                return true;
            if (location.Row == ChessPieceLocation.Row - 1)
                if (location.Column == ChessPieceLocation.Column - 1 || location.Column == ChessPieceLocation.Column || location.Column == ChessPieceLocation.Column + 1)
                    return true;
            if (location.Row == ChessPieceLocation.Row)
                if (location.Column == ChessPieceLocation.Column - 1 || location.Column == ChessPieceLocation.Column + 1)
                    return true;
            if (location.Row == ChessPieceLocation.Row + 1)
                if (location.Column == ChessPieceLocation.Column - 1 || location.Column == ChessPieceLocation.Column || location.Column == ChessPieceLocation.Column + 1)
                    return true;
            return false;
        }
        public bool IsAbleCastling(ChessGame board, Location location)
        {
            if (!AbleCastling)
                return false;
            if (board.IsInCheck(IsWhite))
                return false;
            if (smallCastling(board,location))
                return true;
            if (bigCastling(board,location))
                return true;
            return false;
        }
        public bool smallCastling(ChessGame board,Location location)
        {
            bool isCastleWassuccessful = false;
            int castlingRow = IsWhite ? 7 : 0;
            if (location.Row == castlingRow && location.Column == 6)
            {
                if (board.ChessBoard[castlingRow, 7] is Rook && board.ChessBoard[castlingRow, 7].IsWhite == IsWhite && ((Rook)board.ChessBoard[castlingRow, 7]).AbleCastling)
                {
                    if (board.ChessBoard[castlingRow, 6] == null && board.ChessBoard[castlingRow, 5] == null)
                    {
                        board.ChessBoard[castlingRow, 5] = this;
                        this.ChessPieceLocation = new Location { Row = castlingRow, Column = 5 };
                        board.ChessBoard[ChessPieceLocation.Row, 4] = null;
                        if (!board.IsInCheck(IsWhite))
                        {
                            board.ChessBoard[castlingRow, 6] = this;
                            this.ChessPieceLocation = new Location { Row = castlingRow, Column = 6 };
                            board.ChessBoard[castlingRow, 5] = null;
                            if (!board.IsInCheck(IsWhite))
                                isCastleWassuccessful = true;
                        }
                        board.ChessBoard[castlingRow, 4] = this;
                        this.ChessPieceLocation= new Location { Row = castlingRow, Column = 4 };
                        board.ChessBoard[castlingRow, 5] = null;
                        board.ChessBoard[castlingRow, 6] = null;
                    }
                }
            }
            return isCastleWassuccessful;
        }
        public bool bigCastling(ChessGame board, Location location)
        {
            bool isCastleWassuccessful = false;
            int castlingRow = IsWhite ? 7 : 0;
            if (location.Row == castlingRow && location.Column == 2)
            {
                if (board.ChessBoard[castlingRow, 0] is Rook && board.ChessBoard[castlingRow, 0].IsWhite == IsWhite && ((Rook)board.ChessBoard[castlingRow, 0]).AbleCastling)
                {
                    if (board.ChessBoard[castlingRow, 1] == null && board.ChessBoard[castlingRow, 2] == null && board.ChessBoard[castlingRow,3] == null)
                    {
                        board.ChessBoard[castlingRow, 2] = this;
                        this.ChessPieceLocation = new Location { Row = castlingRow, Column = 2 };
                        board.ChessBoard[castlingRow, 4] = null;
                        if (!board.IsInCheck(IsWhite))
                        {
                            board.ChessBoard[castlingRow, 3] = this;
                            this.ChessPieceLocation = new Location { Row = castlingRow, Column = 3 };
                            board.ChessBoard[castlingRow, 2] = null;
                            if (!board.IsInCheck(IsWhite))
                                isCastleWassuccessful = true;
                        }
                        board.ChessBoard[castlingRow, 4] = this;
                        this.ChessPieceLocation=new Location { Row = castlingRow, Column = 4 };
                        board.ChessBoard[castlingRow, 2] = null;
                        board.ChessBoard[castlingRow, 3] = null;
                    }
                }
            }
            return isCastleWassuccessful;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            King other = obj as King;
            if (other == null)
                return false;
            return base.Equals(obj) && AbleCastling == other.AbleCastling;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ AbleCastling.GetHashCode();
        }
    }
    public struct Location
    {
        public int Row;
        public int Column;
        public static bool operator==(Location x, Location y)
        {
            return x.Row == y.Row && x.Column == y.Column;
        }
        public static bool operator !=(Location x, Location y)
        {
            return x.Row != y.Row || x.Column != y.Column;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Location other = (Location)obj;
            return this == other;
        }
        public override int GetHashCode()
        {
            return Row.GetHashCode()^Column.GetHashCode();
        }
    }
}

