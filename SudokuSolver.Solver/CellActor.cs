using Akka.Actor;
using SudokuSolver.Common.Messages;
using System;
using System.Collections.Generic;

namespace SudokuSolver.Solver
{
    public class CellActor : ReceiveActor
    {
        public class StartWorkingMessage { }
        private int rowIndex;
        private int columnIndex;
        private List<int> possibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private IActorRef printerActor;
        private int? cellNumber;
        private List<IActorRef> rowCells = new List<IActorRef>();
        private List<IActorRef> columnCells = new List<IActorRef>();
        private List<IActorRef> squareCells = new List<IActorRef>();
        public CellActor(IActorRef printerActor, int rowIndex, int columnIndex) : this(printerActor, rowIndex, columnIndex, null)
        {
        }

        private void StartWork()
        {
            Console.WriteLine("I am working!");
        }

        public CellActor(IActorRef printerActor, int rowIndex, int columnIndex, int? cellNumber)
        {
            this.cellNumber = cellNumber;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.printerActor = printerActor;
            Receive<StartWorkingMessage>(_ => StartWork());
            Receive<StartHandShakeMessage>(_ => StartHandshake());
            Receive<HandshakeRequestMessage>(request => ProcessHandhakeRequest(request));
            Receive<HandshakeResponseMessage>(response => ProcessHandhakeResponse(response));

            printerActor.Tell(new PrintCellValueMessage(rowIndex, columnIndex, cellNumber));
        }

        private void ProcessHandhakeResponse(HandshakeResponseMessage response)
        {
            switch (response.Location)
            {
                case "row":
                    rowCells.Add(response.Myself);
                    break;
                case "column":
                    rowCells.Add(response.Myself);
                    break;
                case "square":
                    squareCells.Add(response.Myself);
                    break;
            }
        }

        private void ProcessHandhakeRequest(HandshakeRequestMessage request)
        {
            Sender.Tell(new HandshakeResponseMessage(Self, request.Location));
        }

        private void StartHandshake()
        {
            Context.ActorSelection($"../Cell-{rowIndex}-*").Tell(new HandshakeRequestMessage("row"));
            Context.ActorSelection($"../Cell-{rowIndex}-*").Tell(new HandshakeRequestMessage("column"));

        }

        public static Props Props(IActorRef printerActor, int rowIndex, int columnIndex)
        {
            return Akka.Actor.Props.Create(() => new CellActor(printerActor, rowIndex, columnIndex));
        }

        public static Props Props(IActorRef printerActor, int rowIndex, int columnIndex, int cellValue)
        {
            return Akka.Actor.Props.Create(() => new CellActor(printerActor, rowIndex, columnIndex, cellValue));
        }
    }
}
