using AirportTicketBooking.Models;
using AirportTicketBooking.Repository;

namespace AirportTicketBooking.CSVFiles
{
    public static class FlightService
    {
        public static List<FlightDTO> GetAll() => FlightsRepository.Flights;

        public static FlightDTO GetById(int flightID)
        {
            if (!Exists(flightID))
            {
                throw new Exception($"The flight of ID {flightID} doesn't exist.");
            }

            return GetAll().Single(flight => flight.Id == flightID);
        }

        public static void Add(FlightDTO newFlight) => FlightsRepository.Add(newFlight);

        public static void RemoveAll() => FlightsRepository.Delete();

        public static void ImportCSVFile(string flightsFilePath)
        {
            FlightsRepository.LoadFromFile(flightsFilePath);
        }

        public static List<FlightDTO> GetAllAvailable()
        {
            return GetAll().Where(flight => IsAvailableForBooking(flight))
                .ToList();
        }

        public static bool IsAvailableForBooking(FlightDTO flight)
        {
            if (!BookingService.IsEmpty())
            {
                var numberOfPassengers = BookingService.GetAll()
                    .Where(booking => booking.FlightId == flight.Id)
                    .Count();

                return numberOfPassengers < flight.AirplaneCapacity;
            }
            else return true;
        }

        public static bool Exists(int flightID)
        {
            if (IsEmpty())
                return false;

            return GetAll().Any(flight => flight.Id == flightID);
        }

        public static List<FlightDTO> FilterByPrice(List<FlightDTO> filteredAvailableFlights,
            decimal price)
        {
            return filteredAvailableFlights.Where(flight => flight.Price == price).ToList();
        }

        public static List<FlightDTO> FilterByDepartureCountry(List<FlightDTO> filteredAvailableFlights,
            string? departureCountry)
        {
            return filteredAvailableFlights.Where(flight => 
                flight.DepartureCountry == departureCountry).ToList();
        }

        public static List<FlightDTO> FilterByDestinationCountry(List<FlightDTO> filteredAvailableFlights,
            string? destinationCountry)
        {
            return filteredAvailableFlights.Where(flight =>
                flight.DestinationCountry == destinationCountry).ToList();
        }

        public static List<FlightDTO> FilterByDepartureDate(List<FlightDTO> filteredAvailableFlights,
            DateTime departureDate)
        {
            return filteredAvailableFlights.Where(flight =>
                flight.DepartureDate == departureDate).ToList();
        }

        public static List<FlightDTO> FilterByDepartureAirport(List<FlightDTO> filteredAvailableFlights,
            string? departureAirport)
        {
            return filteredAvailableFlights.Where(flight =>
                flight.DepartureAirport == departureAirport).ToList();
        }

        public static List<FlightDTO> FilterByArrivalAirport(List<FlightDTO> filteredAvailableFlights,
            string? arrivalAirport)
        {
            return filteredAvailableFlights.Where(flight =>
                flight.ArrivalAirport == arrivalAirport).ToList();
        }

        public static List<FlightDTO> FilterByClass(List<FlightDTO> filteredAvailableFlights,
            FlightClass flightClass)
        {
            return filteredAvailableFlights.Where(flight =>
                flight.Class == flightClass).ToList();
        }

        public static bool IsEmpty() => !GetAll().Any();

    }
}