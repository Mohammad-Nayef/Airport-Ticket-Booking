using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class PassengerDTO
    {
        private static int NextUniqueId { get; set; } = 1;

        public int Id { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// Creates and stores the passenger in passengers file.
        /// </summary>
        public PassengerDTO(string name)
        {
            Name = name;
            if (PassengerService.HasData)
            {
                NextUniqueId = PassengerService.GetAll().Last().Id + 1;
            }

            Id = NextUniqueId;
            PassengerService.Append(this);
            NextUniqueId++;
        }

        public PassengerDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"Passenger -> ID: {Id}, Name: {Name}";
    }
}