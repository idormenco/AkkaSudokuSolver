using System;
using System.Collections.Generic;
using Akka.Actor;
using SudokuSolver.Common.Messages;
using System.Linq;
using Akka.Util.Internal;

namespace SudokuSolver.Solver
{
    class GameCoordinatorActor : TypedActor,
        IHandle<IAmPrinterMessage>,
        IHandle<HandshakeDoneMessage>,
        IHandle<PrintCluesMessage>,
        IHandle<PrintCluesResponseMessage>
    {
        private IActorRef Printer;
        private readonly int[,] gameBoard;
        private string[,] cluesBoard;
        private readonly List<IActorRef> unsolvedActorRefs = new List<IActorRef>();
        private readonly List<IActorRef> solvedActorRefs = new List<IActorRef>();
        public GameCoordinatorActor(int[,] gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public GameCoordinatorActor()
        {
            gameBoard = new int[,]
            {
                {0,8,1,0,0,7,0,3,4 },
                {2,0,4,8,3,0,0,0,1 },
                {0,3,0,0,9,1,0,2,8 },
                {5,1,8,3,0,0,0,6,0 },
                {3,0,0,7,6,2,1,0,0 },
                {0,6,0,0,0,8,9,4,3 },
                {0,9,0,6,8,0,4,7,0 },
                {4,0,3,0,2,0,8,0,6 },
                {8,0,6,1,0,4,3,0,0 }
            };
            cluesBoard = new string[9, 9];
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new GameCoordinatorActor());
        }
        public static Props Props(int[,] gameBoard)
        {
            return Akka.Actor.Props.Create(() => new GameCoordinatorActor(gameBoard));
        }

        public void Handle(IAmPrinterMessage message)
        {
            System.Console.WriteLine("initializing game");

            Printer = Sender;
            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                for (int y = 0; y < gameBoard.GetLength(1); y++)
                {

                    if (gameBoard[x, y] != 0)
                    {
                        Context.ActorOf(CellActor.Props(Printer, x + 1, y + 1, gameBoard[x, y]), $"Cell-{x + 1}-{y + 1}");
                    }
                    else
                    {
                        Context.ActorOf(CellActor.Props(Printer, x + 1, y + 1), $"Cell-{x + 1}-{y + 1}");
                    }
                }
            }

            Context.GetChildren().ForEach(x => x.Tell(StartHandshakesMessage.Instance));
        }

        public void Handle(HandshakeDoneMessage message)
        {
            if (message.Value.HasValue)
            {
                solvedActorRefs.Add(Sender);
            }
            else
            {
                unsolvedActorRefs.Add(Sender);
            }


            if (solvedActorRefs.Count + unsolvedActorRefs.Count == 81)
            {
                Printer.Tell(new PrintMessage("[coordinator] all handshakes done!"));
                solvedActorRefs.AsParallel().ForAll(x => x.Tell(SendStateMessage.Instance));
            }
        }

        public void Handle(PrintCluesMessage message)
        {
            solvedActorRefs.AsParallel().ForAll(x => x.Tell(message));
        }

        public void Handle(PrintCluesResponseMessage message)
        {
            cluesBoard[message.Row - 1, message.Column - 1] = message.PossibleInts.Select(x => x.ToString()).Join("");

            PrintBoard();
        }


        private void PrintBoard()
        {
            for (int i = 0; i < cluesBoard.GetLength(0); i++)
            {
                if (i == 0) Console.WriteLine("--------+-------+--------");
                string line = "";
                for (int j = 0; j < cluesBoard.GetLength(1); j++)
                {
                    var cell = cluesBoard[i, j];
                    line += (j == 0 ? "| " : "") + (cell != "" ? cell : gameBoard[i, j].ToString()) + " " + ((j + 1) % 3 == 0 ? "| " : "");
                }
                Console.WriteLine(line);
                if ((i + 1) % 3 == 0) Console.WriteLine("--------+-------+--------");

            }
            Console.WriteLine();
        }


        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}
