using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class FlightDTO
    {
        private static int NextUniqueId { get; set; } = 1;

        public int Id { get; private set; }

        [Required]
        [Range(1, 100_000)]
        [DisplayName("Airplane capacity")]
        public int AirplaneCapacity { get; set; }

        [Required]
        [Range(0, (double)Decimal.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Departure country")]
        public string? DepartureCountry { get; set; }

        [Required]
        [FutureDate]
        [DisplayName("Departure date")]
        public DateTime DepartureDate { get; set; }

        [StringLength(100)]
        [DisplayName("Departure airport")]
        public string? DepartureAirport { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Destination country")]
        public string? DestinationCountry { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Arrival airport")]
        public string? ArrivalAirport { get; set; }

        [Required]
        public FlightClass Class { get; set; }

        private FlightDTO(int airplaneCapacity, decimal price, string? departureCountry,
            DateTime departureDate, string? departureAirport, string? destinationCountry,
            string? arrivalAirport, FlightClass @class)
        {
            AirplaneCapacity = airplaneCapacity;
            Price = price;
            DepartureCountry = departureCountry;
            DepartureDate = departureDate;
            DepartureAirport = departureAirport;
            DestinationCountry = destinationCountry;
            ArrivalAirport = arrivalAirport;
            Class = @class;
        }

        public FlightDTO(int airplaneCapacity, decimal price, string? departureCountry,
            DateTime departureDate, string? departureAirport, string? destinationCountry,
            string? arrivalAirport, FlightClass @class, bool validation = false) : this (airplaneCapacity,  price, departureCountry,
                departureDate, departureAirport, destinationCountry,
                arrivalAirport, @class)
        {
            if (!validation)
            {
                Id = NextUniqueId;

                if (FlightService.Exists(Id))
                {
                    throw new Exception($"The flight of ID {Id} already exists.");
                }

                FlightService.Add(this);
                NextUniqueId++;
            }
        }

        public FlightDTO(int id, int airplaneCapacity, decimal price, string? departureCountry,
            DateTime departureDate, string? departureAirport,
            string? destinationCountry, string? arrivalAirport, FlightClass @class) : this(airplaneCapacity, price, departureCountry,
                departureDate, departureAirport, destinationCountry,
                arrivalAirport, @class)
        {
            Id = id;
        }

        public override string ToString() => $"Flight -> ID: {Id}, Airplane capacity: " +
            $"{AirplaneCapacity}, Price: {Price}, Departure country: {DepartureCountry}, " +
            $"\nDeparture date: {DepartureDate.Month}-{DepartureDate.Day}-{DepartureDate.Year}, " +
            $"Departure airport: {DepartureAirport}, Destination country: {DestinationCountry}, " +
            $"\nArrival airport: {ArrivalAirport}, Class: {Class}";
    }
}