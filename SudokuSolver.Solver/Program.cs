using System;
using Akka.Actor;
using Akka.Configuration;

namespace SudokuSolver.Solver
{
    class Program
    {
        private static ActorSystem actorSystem;

        static void Main(string[] args)
        {
            var initialGame = new int[,]
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

            actorSystem.ActorOf(GameCoordinatorActor.Props(),"Sudoku");
            


            actorSystem.WhenTerminated.Wait();
        }
    }
}
