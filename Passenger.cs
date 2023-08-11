using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking
{
    public class Passenger
    {
        public static int NextUniqueID { get; private set; } = 1;

        public int ID { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// Creates and stores the passenger in passengers file.
        /// </summary>
        public Passenger(string name)
        {
            Name = name;
            if (PassengersFile.HasData)
            {
                NextUniqueID = PassengersFile.GetAll().Last().ID + 1;
            }

            ID = NextUniqueID;
            PassengersFile.Append(this);
            NextUniqueID++;
        }

        public Passenger(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString() => $"Passenger -> ID: {ID}, Name: {Name}";
    }
}