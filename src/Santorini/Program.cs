//using System;

//namespace Santorini
//{
//    class Program
//    {
//        private static readonly string Player1 = "Black";
//        private static readonly string Player2 = "White";

//        static void Main(string[] args)
//        {
//            //Start Game
//            var game = new Game();

//            //Add Players
//            game.AddPlayer(Player1);
//            game.AddPlayer(Player2);

//            //Add first Figures
//            game.AddFigure(Player1, 0, 0);
//            game.AddFigure(Player2, 4, 4);

//            //Add second Figures
//            game.AddFigure(Player1, 1, 1);
//            game.AddFigure(Player2, 3, 3);

//            //Move
//            Console.WriteLine(game.TryMove(new MoveCommand(Player1, 0, new Land(1, 0), new Land(0, 0))));
//            Console.WriteLine(game.TryMove(new MoveCommand(Player2, 1, new Land(3, 2), new Land(0, 3))));

//            Console.ReadLine();
//        }
//    }
//}
