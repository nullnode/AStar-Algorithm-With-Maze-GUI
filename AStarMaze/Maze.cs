using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class Maze : Form
    {
        public Maze()
        {
            InitializeComponent();
            PictureBox beginPos = pictureBox18;
            beginPos.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Location start = new Location { X = 1, Y = 2 }; // starting position on the X and Y coordinates
            Location target = new Location { X = 2, Y = 5 }; // goal position
            FindPathAsync(start, target);
        }

        public async Task FindPathAsync(Location start, Location target)
        {
            // used as a map where A represents the initial location and B represents the goal state. X's are walls.
            string[] map = new string[]
{
                "+------+",
                "|      |",
                "|A X   |",
                "|XXX   |",
                "|   X  |",
                "| B    |",
                "|      |",
                "+------+",
};

            // 2D array representing the visualization of the map, the names are irrelevant since we're using the X and Y coords
            PictureBox[,] area = new PictureBox[8, 8]
            {
            { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8},
            { pictureBox9, pictureBox10 , pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16},
            { pictureBox17, pictureBox18, pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24},
            { pictureBox25, pictureBox26, pictureBox27, pictureBox28, pictureBox29, pictureBox30, pictureBox31, pictureBox32},
            { pictureBox33, pictureBox34, pictureBox35, pictureBox36, pictureBox37, pictureBox38, pictureBox39, pictureBox40},
            { pictureBox41, pictureBox42, pictureBox43, pictureBox44, pictureBox45, pictureBox46, pictureBox47, pictureBox48},
            { pictureBox49, pictureBox50, pictureBox51, pictureBox52, pictureBox53, pictureBox54, pictureBox55, pictureBox56},
            { pictureBox57, pictureBox58, pictureBox59, pictureBox60, pictureBox61, pictureBox62, pictureBox63, pictureBox64}
            };

            Location current = null;
            List<Location> openList = new List<Location>();
            List<Location> closedList = new List<Location>();
            int g = 0;
            openList.Add(start); // start off by adding the rats current (first) position to the open list

            while (openList.Count > 0)
            {
                int lowest = openList.Min(l => l.F); 
                current = openList.First(l => l.F == lowest); 

                closedList.Add(current); // current square gets moved to the closedList

                textBox1.AppendText("Considering row: " + current.Y + " at column: " + current.X + "\r\n");
                PictureBox show = area[current.Y, current.X]; //show the square under consideration for movement
                show.BackgroundImage = think.BackgroundImage;
                show.Visible = true;

                await Task.Delay(1000);
                openList.Remove(current); // remove current square from the open list

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                // used for holding the adjacent squares
                List<Location> adjacentSquares = PotentialSquares(current.X, current.Y, map);
                g++;

                foreach (Location adjacentSquare in adjacentSquares)
                {
                    // check to see if nearby squares are in the closed list, if so then they're ignored
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // check to see if its in the open list, if not then we'll find the cost and add it to the open list
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        adjacentSquare.G = g;
                        adjacentSquare.H = HScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // we want the lowest score possible, so lets see if the current g score can bring down the adjacent
                        // squares f score. if it does then we'll update the parent and our path will be more efficient
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            // path has been found, so lets push all our locations onto a stack (along with their coordinates for logging purposes)
            // then we can pop them all off and show the rat moving to his destination!
            Stack<PictureBox> path = new Stack<PictureBox>();
            Stack<Location> choice = new Stack<Location>();
            while (current != null)
            {
                PictureBox moveHere = area[current.Y, current.X];
                path.Push(moveHere);
                choice.Push(current);
                current = current.Parent;
            }
            while(path != null)
            {
                PictureBox move = path.Pop();
                move.BackgroundImage = rat.BackgroundImage;
                move.Visible = true;

                Location ch = choice.Pop();
                textBox1.AppendText("Path from row: " + ch.Y + " at column: " + ch.X + "\r\n");
                await Task.Delay(1000);
            }
        }

        // using the 1D map array to propose locations based on spaces and the goal (walkable areas)
        // x and y are used to indicate the potential areas of movement since we're seeing if we can move up/down/left/right
        static List<Location> PotentialSquares(int x, int y, string[] map)
        {
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };

            return proposedLocations.Where(l => map[l.Y][l.X] == ' ' || map[l.Y][l.X] == 'B').ToList();
        }

        // find the h score of an adjacent square
        static int HScore(int x, int y, int targetX, int targetY)
        {
            int h = Math.Abs(targetX - x) + Math.Abs(targetY - y);
            return h;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // To do: Randomize the maze!
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    // location class used for storing the x and y coordinates, heuristic values, and a parent node for backtracking
    public class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }
}
