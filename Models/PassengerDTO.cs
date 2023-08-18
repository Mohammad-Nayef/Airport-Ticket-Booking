using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class PassengerDTO
    {
        private static int _nextUniqueId = 1;
        private static int NextUniqueId 
        { 
            get
            {
                if (!PassengerService.IsEmpty())
                    return _nextUniqueId = PassengerService.GetAll().Last().Id + 1;

                return _nextUniqueId;
            }
            set
            {
                _nextUniqueId = value;
            } 
        }

        public int Id { get; private set; }

        public string Name { get; set; }

        /// <summary>
        /// Creates and stores the passenger in passengers file.
        /// </summary>
        public PassengerDTO(string name)
        {
            Name = name;
            Id = NextUniqueId;
            PassengerService.Add(this);
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