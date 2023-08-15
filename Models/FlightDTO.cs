using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
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

        /// <summary>
        /// Creates and stores the flight in flights file.
        /// </summary>
        public FlightDTO(int airplaneCapacity, decimal price, string? departureCountry,
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

            Id = NextUniqueId;
            FlightService.Append(this);
            NextUniqueId++;
        }

        public FlightDTO(int id, int airplaneCapacity, decimal price, string? departureCountry,
                      DateTime departureDate, string? departureAirport,
                      string? destinationCountry, string? arrivalAirport, FlightClass @class)
        {
            Id = id;
            AirplaneCapacity = airplaneCapacity;
            Price = price;
            DepartureCountry = departureCountry;
            DepartureDate = departureDate;
            DepartureAirport = departureAirport;
            DestinationCountry = destinationCountry;
            ArrivalAirport = arrivalAirport;
            Class = @class;
        }

        public static bool IsValid(FlightDTO flight)
        {
            var properties = flight.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(flight);
                var attributes = property.GetCustomAttributes();

                foreach (var attribute in attributes)
                {
                    if (attribute is FutureDateAttribute futureDate && !futureDate.IsValid(value) ||
                        attribute is RequiredAttribute required && !required.IsValid(value) ||
                        attribute is RangeAttribute range && !range.IsValid(value) ||
                        attribute is StringLengthAttribute stringLength && !stringLength.IsValid(value))
                    {
                        NextUniqueId--;
                        return false;
                    }
                }
            }

            return true;
        }

        public static string GetConstraints()
        {
            var constraints = new StringBuilder("Flights data must be stored like this:\n" +
                "Airplane capacity, Price, Departure country, Departure date (MM-DD-YYYY), " +
                "Departure airport, Destination country, Arrival airport, Case sensitive " +
                "class (Economy, Business, First)\n\n");
            var properties = typeof(FlightDTO).GetProperties();

            foreach (var property in properties)
            {
                var displayName = property.GetCustomAttributes()
                    .FirstOrDefault(attribute => attribute is DisplayNameAttribute);

                if (displayName != null)
                {
                    var name = displayName as DisplayNameAttribute;
                    constraints.AppendLine($"{name.DisplayName}:");
                }
                else if (property.GetCustomAttributes().Count() == 0)
                {
                    continue;
                }
                else
                {
                    constraints.AppendLine($"{property.Name}:");
                }

                var hasRequiredAttribute = property.GetCustomAttributes()
                    .Any(attribute => attribute is RequiredAttribute);

                if (hasRequiredAttribute)
                    constraints.AppendLine("Required");
                
                var stringLength = property.GetCustomAttributes()
                    .FirstOrDefault(attribute => attribute is StringLengthAttribute);

                if (stringLength == null)
                {
                    constraints.AppendLine("Free text");
                }
                else
                {
                    var maxLength = (stringLength as StringLengthAttribute).MaximumLength;
                    constraints.AppendLine($"Text of maximum length = {maxLength}");
                }
                
                if (property.GetType() == typeof(sbyte) ||
                    property.GetType() == typeof(ushort) ||
                    property.GetType() == typeof(uint) ||
                    property.GetType() == typeof(ulong))
                {
                    constraints.AppendLine("Positive number");
                }

                var numberRange = property.GetCustomAttributes()
                    .FirstOrDefault(attribute => attribute is RangeAttribute);

                if (numberRange != null)
                {
                    var range = numberRange as RangeAttribute;
                    constraints.AppendLine($"Valid range: {range.Minimum} to {range.Maximum}");
                }

                var isFutureDate = property.GetCustomAttributes()
                    .Any(attribute => attribute is FutureDateAttribute);

                if (isFutureDate)
                    constraints.AppendLine("Future date");

                constraints.AppendLine();
            }
            return constraints.ToString();
        }

        public override string ToString() => $"Flight -> ID: {Id}, Airplane capacity: " +
            $"{AirplaneCapacity}, Price: {Price}, Departure country: {DepartureCountry}, " +
            $"\nDeparture date: {DepartureDate.Month}-{DepartureDate.Day}-{DepartureDate.Year}, " +
            $"Departure airport: {DepartureAirport}, Destination country: {DestinationCountry}, " +
            $"\nArrival airport: {ArrivalAirport}, Class: {Class}";
    }
}

public enum FlightClass
{
    Economy,
    Business,
    First
}