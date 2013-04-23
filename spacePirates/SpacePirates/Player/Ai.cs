using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Ai : IPlayer
    {
        Ownership ownerLink;
        String name;
        private static List<String> names;
        private static int nameIndex;

        public Ai()
        {
            name = GetAiName();
        }

        public Ai(String name)
        {
            this.name = name;
        }

        public String GetName()
        {
            return name;
        }

        public ISpaceShip GetShip()
        {
            return ownerLink.GetShip();
        }

        public void SetOwnerShip(Ownership ownerLink)
        {
            this.ownerLink = ownerLink;
        }

        /// <summary>
        /// Determine general goal/strategy
        /// </summary>
        void MakeGeneralPlan()
        {
        }

        /// <summary>
        /// Make local decisions supporting the strategy
        /// </summary>
        void MakeLocalPlan()
        {
        }

         public static IPlayer createController()
        {
            return new Ai("nocomputer"); 
            //throw new NotImplementedException();
        }

        private static String GetAiName()
        {
            if (names == null)
            {
                //load the files if they haven't been loaded yet
                SetupNames();
            }

            String name = "Unnamed Ai";          
            if (nameIndex < names.Count)
            {
                //Requested more names than expected
                name = names[nameIndex];
                nameIndex++;
            }
            return name;
        }

        private static void SetupNames()
        {
            List<String> tempNames = new List<String>(30);

            int counter = 0;
            string line;

            // Read the names from a file
            String path = AppDomain.CurrentDomain.BaseDirectory + "Content/Text/AiNames.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                tempNames.Add(line);
                Console.WriteLine("Read name: " + line);
                counter++;
            }

            file.Close();

            //grab ten random names
            Random rnd = new Random();
            names = (List<String>)tempNames.OrderBy(x => rnd.Next()).Take(10).ToList<String>();
            nameIndex = 0;
        }


        public bool WasShipUpgraded()
        {
            throw new NotImplementedException();
        }

        public void ShipUpgraded()
        {
            throw new NotImplementedException();
        }
        public int GetCurrency()
        {
            return 0;
        }
    }
}
