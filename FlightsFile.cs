using System.Text;

namespace AirportTicketBooking.CSVFiles
{
    public static class FlightsFile
    {
        private const string FlightsFilePath = "Flights.csv";

        public static bool HasData
        {
            get
            {
                var fileInfo = new FileInfo(FlightsFilePath);
                return fileInfo.Exists && fileInfo.Length > 0;  
            }
        }

        public static void Append(FlightDTO newFlight)
        {
            if (Exists(newFlight.ID))
                throw new Exception("This flight already exists.");

            File.AppendAllText(FlightsFilePath, $"{newFlight.ID}, " +
                $"{newFlight.AirplaneCapacity}, {newFlight.Price}, " +
                $"{newFlight.DepartureCountry}, {newFlight.DepartureDate.Month}-" +
                $"{newFlight.DepartureDate.Day}-{newFlight.DepartureDate.Year}, " +
                $"{newFlight.DepartureAirport}, {newFlight.DestinationCountry}, " +
                $"{newFlight.ArrivalAirport}, {newFlight.Class}\n");
        }

        public static void ImportCSVFile(string newFlightsFilePath)
        {
            try
            {
                RemoveAll();
                IsValid(newFlightsFilePath);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public static bool IsValid(string newFilePath)
        {
            var _fileReader = new StreamReader(newFilePath);
            bool errorFound = false;
            var errorLines = new StringBuilder($"Found errors at these lines in " +
                $"{Path.GetFileName(newFilePath)}:\n");

            for (var line = 1; !_fileReader.EndOfStream; line++)
            {
                var newLine = _fileReader.ReadLine();
                if (String.IsNullOrEmpty(newLine))
                    continue;

                var newFlight = newLine.Split(", ");

                if (!int.TryParse(newFlight[0], out var airplaneCapacity) ||
                    !decimal.TryParse(newFlight[1], out var price) ||
                    !DateTime.TryParse(newFlight[3], out DateTime departureDate) ||
                    !Enum.TryParse(typeof(FlightClass), newFlight[7], out var flightClass) ||
                    !FlightDTO.IsValid(new FlightDTO(airplaneCapacity, price, newFlight[2],
                    departureDate, newFlight[4], newFlight[5], newFlight[6],
                    (FlightClass)flightClass)))
                {
                    errorFound = true;
                    errorLines.Append($"{line}, "); 
                }
            }

            if (errorFound)
            {
                RemoveAll();
                throw new Exception(errorLines.ToString() +
                    "\nMake sure that you follow the constraints.");
            }

            _fileReader.Close();
            return true;
        }

        public static List<FlightDTO> GetAll()
        {
            var flightsList = new List<FlightDTO>();
            var _fileReader = new StreamReader(FlightsFilePath);

            while (!_fileReader.EndOfStream)
            {
                var flightData = _fileReader.ReadLine()?.Split(", ");

                // Flights CSV file format: ID, Airplane capacity, Price,
                // Departure country, Departure date, Departure airport,
                // Destination country, Arrival airport, Class
                flightsList.Add(new FlightDTO(int.Parse(flightData[0]), 
                    int.Parse(flightData[1]), int.Parse(flightData[2]),
                    flightData[3], DateTime.Parse(flightData[4]), flightData[5],
                    flightData[6], flightData[7], 
                    (FlightClass)Enum.Parse(typeof(FlightClass), flightData[8])));
            }

            _fileReader.Close ();
            return flightsList;
        }

        public static FlightDTO Get(int flightID)
        {
            if (!Exists(flightID))
                throw new Exception($"The flight of ID: {flightID} doesn't exist.");

            return GetAll().Single(flight => flight.ID == flightID);
        }

        public static List<FlightDTO> GetAllAvailable()
        {
            if (!HasData)
                throw new Exception("There are no flights.");

            return GetAll().Where(flight =>
                {
                    if (BookingsFile.HasData)
                    {
                        var numberOfPassengers = BookingsFile.GetAll()
                            .Where(booking => booking.Flight.ID == flight.ID)
                            .Count();

                        return numberOfPassengers < flight.AirplaneCapacity;
                    }
                    else return true;
                }).ToList();
        }

        public static void RemoveAll()
        {
            if (HasData)
                File.Delete(FlightsFilePath);
        }

        public static bool Exists(int flightID)
        {
            if (HasData)
                return GetAll().Any(flight => flight.ID == flightID);

            return false;
        }
    }
}