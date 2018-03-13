using System;
using Akka.Actor;
using Akka.Configuration;
using SudokuSolver.Common.Messages;

namespace SudokuSolver.Solver
{
    class Program
    {
        private static ActorSystem actorSystem;

        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                            port = 8081
                            hostname = 0.0.0.0
                            public-hostname = localhost
                        }
                    }
                }
            ");

            actorSystem = ActorSystem.Create("SudokuSolverActorSystem", config);

            IActorRef gameActor = actorSystem.ActorOf(GameCoordinatorActor.Props(), "Sudoku");

            actorSystem.WhenTerminated.Wait();


            while (true)
            {
                string command = Console.ReadLine() ?? "";

                switch (command.ToLower().Trim())
                {
                    case "exit":
                        System.Environment.Exit(1);
                        break;
                    case "print":
                        gameActor.Tell(PrintCluesMessage.Instance);
                        break;
                }
            }

        }
    }
}
