using Akka.Actor;
using SudokuSolver.Common.Messages;
using System;

namespace SudokuSolver.Printer
{
    public class PrinterActor : TypedActor,
        IHandle<FindSudokuGameMessage>,
        IHandle<IHaveThisNumber>,
        IHandle<PrintMessage>
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://SudokuSolverActorSystem@localhost:8081/user/Sudoku");
        private int valuesLeft = 81;
        private readonly int?[,] sudokuBoard = new int?[9, 9];

        public PrinterActor()
        {
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new PrinterActor());
        }

        public void Handle(IHaveThisNumber message)
        {
            sudokuBoard[message.Row - 1, message.Column - 1] = message.Number;
            Console.Clear();
            PrintBoard();
            --valuesLeft;
            if (valuesLeft == 0)
            {
                Console.WriteLine("DONE!");
            }
        }

        private void PrintBoard()
        {
            for (int i = 0; i < sudokuBoard.GetLength(0); i++)
            {
                if (i == 0) Console.WriteLine("--------+-------+--------");
                string line = "";
                for (int j = 0; j < sudokuBoard.GetLength(1); j++)
                {
                    int? cell = sudokuBoard[i, j];
                    line += (j == 0 ? "| " : "") + (cell.HasValue ? cell.ToString() : "_") + " " + ((j + 1) % 3 == 0 ? "| " : "");
                }
                Console.WriteLine(line);
                if ((i + 1) % 3 == 0) Console.WriteLine("--------+-------+--------");

            }
            Console.WriteLine();
        }

        public void Handle(FindSudokuGameMessage message)
        {
            _server.Tell(new IAmPrinterMessage());
        }

        public void Handle(PrintMessage message)
        {
            Console.WriteLine(message.Message);
        }
    }
}
