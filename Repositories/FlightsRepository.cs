using AirportTicketBooking.Models;

namespace AirportTicketBooking.Repository
{
    public static class FlightsRepository
    {
        private const string FlightsFilePath = "Flights.csv";
        private static bool _loaded = false;
        private static List<FlightDTO> _flights = new();

        public static List<FlightDTO> Flights
        {
            get
            {
                if (!_loaded && File.Exists(FlightsFilePath))
                {
                    _loaded = true;
                    _flights = LoadFromFile(FlightsFilePath, true);
                }

                return _flights;
            }
        }

        public static List<FlightDTO> LoadFromFile(string flightsFilePath, bool hasId = false)
        {
            if (!FlightValidator.IsValidFile(flightsFilePath, hasId))
            {
                throw new Exception($"Found errors at these lines in " +
                    $"{Path.GetFileName(flightsFilePath)}:\n{FlightValidator.ErrorsLines}\n" +
                    $"Make sure that you follow the constraints.");
            }

            var fileReader = new StreamReader(flightsFilePath);
            var flights = new List<FlightDTO>();

            while (!fileReader.EndOfStream)
            {
                var index = 0;

                if (hasId)
                    index = 1;

                var flightData = fileReader.ReadLine()?.Split(", ");
                var airplaneCapacity = int.Parse(flightData[index++]);
                var price = decimal.Parse(flightData[index++]);
                var departureCountry = flightData[index++];
                var departureDate = DateTime.Parse(flightData[index++]);
                var departureAirport = flightData[index++];
                var destinationCountry = flightData[index++];
                var arrivalAirport = flightData[index++];
                var flightClass = (FlightClass)Enum.Parse(typeof(FlightClass), flightData[index++]);

                if (hasId)
                {
                    var id = int.Parse(flightData[0]);

                    flights.Add(new FlightDTO(id, airplaneCapacity, price, departureCountry, departureDate,
                        departureAirport, destinationCountry, arrivalAirport, flightClass));
                }
                else
                {
                    flights.Add(new FlightDTO(airplaneCapacity, price, departureCountry, departureDate,
                        departureAirport, destinationCountry, arrivalAirport, flightClass));
                }
            }

            fileReader.Close();
            return flights;
        }

        public static void Add(FlightDTO newFlight)
        {
            _flights.Add(newFlight);
            StoreInFile(newFlight);
        }

        public static void StoreInFile(FlightDTO newFlight)
        {
            _loaded = true;
            File.AppendAllText(FlightsFilePath, $"{newFlight.Id}, " +
                $"{newFlight.AirplaneCapacity}, {newFlight.Price}, " +
                $"{newFlight.DepartureCountry}, {newFlight.DepartureDate.Month}-" +
                $"{newFlight.DepartureDate.Day}-{newFlight.DepartureDate.Year}, " +
                $"{newFlight.DepartureAirport}, {newFlight.DestinationCountry}, " +
                $"{newFlight.ArrivalAirport}, {newFlight.Class}\n");
        }

        public static void Delete()
        {
            if (File.Exists(FlightsFilePath))
                File.Delete(FlightsFilePath);

            _flights.Clear();
        }
    }
}