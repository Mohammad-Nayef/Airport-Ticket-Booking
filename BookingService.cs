using AirportTicketBooking.Models;
using AirportTicketBooking.Repository;

namespace AirportTicketBooking.CSVFiles
{
    public static class BookingService
    {
        public static List<BookingDTO> GetAll() => BookingsRepository.Bookings;

        public static BookingDTO GetById(int bookingId)
        {
            if (!Exists(bookingId))
            {
                throw new Exception($"The booking of ID {bookingId} doesn't exist.");
            }

            return GetAll().Single(booking => booking.Id == bookingId);
        }

        public static void Add(BookingDTO newBooking)
        {
            if (Exists(newBooking.Id))
            {
                throw new Exception($"The booking of ID {newBooking.Id} already exists.");
            }

            BookingsRepository.Add(newBooking);
        }

        public static void RemoveAll() => BookingsRepository.Delete();

        public static List<BookingDTO> GetBookingsOf(int passengerId)
        {
            return GetAll()
                .Where(booking => booking.PassengerId == passengerId)
                .ToList();
        }

        public static void RemoveById(int bookingId)
        {
            if (!Exists(bookingId))
            {
                throw new Exception($"The booking of ID {bookingId} doesn't exist.");
            }

            var modifiedBookings = GetAll()
                .Where(booking => booking.Id != bookingId)
                .ToList();
            BookingsRepository.Overwrite(modifiedBookings);
        }

        public static List<BookingDTO> FilterByPrice(List<BookingDTO> bookings, decimal price)
        {
            return bookings.Where(booking => 
                            FlightService.GetById(booking.FlightId).Price == price)
                            .ToList();
        }

        public static List<BookingDTO> FilterByDepartureCountry(List<BookingDTO> bookings,
            string departureCountry)
        {
            return bookings.Where(booking => 
                            FlightService.GetById(booking.FlightId).DepartureCountry
                                .Equals(departureCountry, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
        }

        public static List<BookingDTO> FilterByDestinationCountry(List<BookingDTO> bookings,
            string destinationCountry)
        {
            return bookings.Where(booking =>
                            FlightService.GetById(booking.FlightId).DestinationCountry
                                .Equals(destinationCountry, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
        }

        public static List<BookingDTO> FilterByDepartureDate(List<BookingDTO> bookings,
            DateTime departureDate)
        {
            return bookings.Where(booking =>
                            FlightService.GetById(booking.FlightId).DepartureDate == departureDate)
                            .ToList();
        }

        public static List<BookingDTO> FilterByDepartureAirport(List<BookingDTO> bookings,
            string departureAirport)
        {
            return bookings.Where(booking =>
                            FlightService.GetById(booking.FlightId).DepartureAirport
                                .Equals(departureAirport, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
        }

        public static List<BookingDTO> FilterByArrivalAirport(List<BookingDTO> bookings,
            string arrivalAirport)
        {
            return bookings.Where(booking =>
                            FlightService.GetById(booking.FlightId).ArrivalAirport
                                .Equals(arrivalAirport, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
        }

        public static List<BookingDTO> FilterByFlightClass(List<BookingDTO> bookings,
            FlightClass flightClass)
        {
            return bookings.Where(booking =>
                            FlightService.GetById(booking.FlightId).Class == flightClass)
                            .ToList();
        }

        public static List<BookingDTO> FilterByFlightId(List<BookingDTO> bookings,
            int flightId)
        {
            return bookings.Join(FlightService.GetAll(),
                            booking => booking.FlightId,
                            flight => flight.Id,
                            (booking, flight) => booking)
                            .Where(booking => booking.FlightId == flightId)
                            .ToList();
        }

        public static List<BookingDTO> FilterByPassengerId(List<BookingDTO> bookings,
            int passengerId)
        {
            return bookings.Join(PassengerService.GetAll(),
                            booking => booking.PassengerId,
                            passenger => passenger.Id,
                            (booking, passenger) => booking)
                            .Where(booking => booking.PassengerId == passengerId)
                            .ToList();
        }

        public static bool Exists(int bookingId)
        {
            return GetAll().Any(booking => booking.Id == bookingId);
        }

        public static bool IsEmpty() => !GetAll().Any();
    }
}