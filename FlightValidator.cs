using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using AirportTicketBooking.Models;
using AirportTicketBooking.Repository;

namespace AirportTicketBooking
{
    public static class FlightValidator
    {
        public static string ErrorsLines { get; private set; } = String.Empty;

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
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsValidFile(string flightsFilePath, bool hasId)
        {
            bool errorFound = false;
            var errorsLines = new StringBuilder();
            var flightsFileData = File.ReadAllLines(flightsFilePath);

            for (var line = 0; line < flightsFileData.Length; line++)
            {
                if (String.IsNullOrEmpty(flightsFileData[line]))
                    continue;

                var newFlight = flightsFileData[line].Split(", ");

                if (hasId)
                {
                    errorFound = !IsValidLineWithId(ref errorsLines, line, newFlight);
                }
                else
                {
                    errorFound = !IsValidLineWithoutId(ref errorsLines, line, newFlight);
                }
            }

            if (errorFound)
            {
                ErrorsLines = errorsLines.ToString();
                return false;
            }

            return true;
        }

        private static bool IsValidLineWithId(ref StringBuilder errorsLines, int line, string[] newFlight)
        {
            if (!int.TryParse(newFlight[0], out var id) ||
                !int.TryParse(newFlight[1], out var airplaneCapacity) ||
                !decimal.TryParse(newFlight[2], out var price) ||
                !DateTime.TryParse(newFlight[4], out DateTime departureDate) ||
                !Enum.TryParse(typeof(FlightClass), newFlight[8], out var flightClass) ||
                !IsValid(new FlightDTO(airplaneCapacity, price, newFlight[3],
                departureDate, newFlight[5], newFlight[6], newFlight[7],
                (FlightClass)flightClass, true)))
            {
                errorsLines.Append($"{line + 1}, ");
                return false;
            }

            return true;
        }

        private static bool IsValidLineWithoutId(ref StringBuilder errorsLines, int line, string[] newFlight)
        {
            if (!int.TryParse(newFlight[0], out var airplaneCapacity) ||
                !decimal.TryParse(newFlight[1], out var price) ||
                !DateTime.TryParse(newFlight[3], out DateTime departureDate) ||
                !Enum.TryParse(typeof(FlightClass), newFlight[7], out var flightClass) ||
                !IsValid(new FlightDTO(airplaneCapacity, price, newFlight[2],
                departureDate, newFlight[4], newFlight[5], newFlight[6],
                (FlightClass)flightClass, true)))
            {
                errorsLines.Append($"{line + 1}, ");
                return false;
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
    }
}
