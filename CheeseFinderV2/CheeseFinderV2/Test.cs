using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CheeseFinderV2
{
    [TestFixture]
    class Test
    {
        [Test]
        public void TestCatMove()
        {
            CheeseNibbler test = new CheeseNibbler();
            Cat cat1 = new Cat();
            cat1.Position = test.Grid[4, 3];
            cat1.Position.Status = Point.PointStatus.Cat;
            test.Cats.Add(cat1);
            Cat cat2 = new Cat();
            cat2.Position = test.Grid[3, 3];
            cat2.Position.Status = Point.PointStatus.Cat;
            test.Cats.Add(cat2);

            //set mouse position
            test.Mouse = new Mouse();
            test.Mouse.Position = test.Grid[2, 3];

            //move the mouse
            test.MoveMouse(ConsoleKey.LeftArrow);
            test.MoveCat(cat1);
            test.MoveCat(cat2);

            //assert the cats are where you want them to be

            Assert.IsTrue(test.Cats[0].Position.X == 4);
            Assert.IsTrue(test.Cats[1].Position.X == 2);


            //CheeseNibbler test = new CheeseNibbler();
            //Cat cat1 = new Cat();
            //cat1.Position = test.Grid[4, 3];
            //cat1.Position.Status = Point.PointStatus.Cat;
            //test.Cats.Add(cat1);
            //Cat cat2 = new Cat();
            //cat2.Position = test.Grid[3, 3];
            //cat2.Position.Status = Point.PointStatus.Cat;
            //test.Cats.Add(cat2);

            ////set mouse position
            ////test.Mouse.Position.Status = Point.PointStatus.Empty;
            //test.Mouse.Position = test.Grid[2, 3];
            //test.Mouse.Position.Status = Point.PointStatus.Mouse;
            //test.DrawGrid();
            //test.MoveMouse(test.GetUserMove());
            //test.MoveCat(cat1);
            //test.MoveCat(cat2);
            //test.DrawGrid();

        }
    }
}
