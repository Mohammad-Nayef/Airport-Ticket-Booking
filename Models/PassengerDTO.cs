using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class PassengerDTO
    {
        private static int NextUniqueId { get; set; } = 1;

        public int ID { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// Creates and stores the passenger in passengers file.
        /// </summary>
        public PassengerDTO(string name)
        {
            Name = name;
            if (PassengersFile.HasData)
            {
                NextUniqueId = PassengersFile.GetAll().Last().ID + 1;
            }

            ID = NextUniqueId;
            PassengersFile.Append(this);
            NextUniqueId++;
        }

        public PassengerDTO(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString() => $"Passenger -> ID: {ID}, Name: {Name}";
    }
}