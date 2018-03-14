using System;
using System.Collections.Generic;
using Akka.Actor;
using SudokuSolver.Common.Messages;
using System.Linq;
using Akka.Event;
using Akka.Util.Internal;

namespace SudokuSolver.Solver
{
    public class GameCoordinatorActor : ReceiveActor
    {
        private IActorRef Printer;
        private readonly int[,] gameBoard;
        private string[,] cluesBoard;
        private readonly List<IActorRef> unsolvedActorRefs = new List<IActorRef>();
        private readonly List<IActorRef> solvedActorRefs = new List<IActorRef>();

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);


        public GameCoordinatorActor(int[,] gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public GameCoordinatorActor()
        {
            gameBoard = new int[,]
            {
                {0,0,0, 0,0,0, 0,3,4 },
                {0,3,5, 0,4,0, 0,0,0 },
                {6,0,0, 0,9,0, 2,0,0 },

                {0,5,6, 0,0,2, 0,1,0 },
                {0,0,0, 0,0,0, 6,0,9 },
                {0,1,8, 0,0,9, 0,2,0 },

                {2,0,0, 0,1,0, 8,0,0 },
                {0,6,9, 0,3,0, 0,0,0 },
                {0,0,0, 0,0,0, 0,4,7 }
            };
            cluesBoard = new string[9, 9];

            Receive<IAmPrinterMessage>(_=> Handle());
            Receive<HandshakeDoneMessage>(message => Handle(message));
            Receive<PrintCluesMessage>(message => Handle(message));
            Receive<IHaveThisNumber>(message => Handle(message));
        }

        private void Handle(IHaveThisNumber message)
        {
            gameBoard[message.Row - 1, message.Column - 1] = message.Number;
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new GameCoordinatorActor());
        }
        public static Props Props(int[,] gameBoard)
        {
            return Akka.Actor.Props.Create(() => new GameCoordinatorActor(gameBoard));
        }

        public void Handle()
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

            Context.GetChildren().ForEach(x => x.Tell(new StartHandshakesMessage()));
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
                solvedActorRefs.AsParallel().ForAll(x => x.Tell(new SendStateMessage()));
            }
        }

        private async void Handle(PrintCluesMessage message)
        {
            unsolvedActorRefs.Clear();
            unsolvedActorRefs.AddRange(Context.GetChildren().ToList());
            foreach (IActorRef actorRef in unsolvedActorRefs)
            {
                PrintCluesResponseMessage response = await actorRef.Ask<PrintCluesResponseMessage>(new PrintCluesMessage());
                Handle(response);
            }
        }

        private void Handle(PrintCluesResponseMessage message)
        {
            string clues = message.PossibleInts.Select(x => x.ToString()).Join("");

            _log.Debug($"received clues for {message.Row} - {message.Column} - {clues}");

            cluesBoard[message.Row - 1, message.Column - 1] = clues;

            PrintBoard();
        }


        private void PrintBoard()
        {
            Console.Clear();
            for (int i = 0; i < cluesBoard.GetLength(0); i++)
            {
                if (i == 0) Console.WriteLine("--------+-------+--------");
                string line = "";
                for (int j = 0; j < cluesBoard.GetLength(1); j++)
                {
                    var cell = cluesBoard[i, j] ?? "";
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
