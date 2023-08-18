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
            return GetAll().Where(booking => booking.PassengerId == passengerId)
                            .ToList();
        }

        public static void RemoveById(int bookingId)
        {
            if (!Exists(bookingId))
            {
                throw new Exception($"The booking of ID {bookingId} doesn't exist.");
            }

            var modifiedBookings = GetAll().Where(booking => booking.Id != bookingId).ToList();
            BookingsRepository.Overwrite(modifiedBookings);
        }

        public static List<BookingDTO> FilterByPrice(List<BookingDTO> bookings, decimal price)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.Price == price;
            }).ToList();
        }

        public static List<BookingDTO> FilterByDepartureCountry(List<BookingDTO> bookings,
            string departureCountry)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.DepartureCountry == departureCountry;
            }).ToList();
        }

        public static List<BookingDTO> FilterByDestinationCountry(List<BookingDTO> bookings,
            string destinationCountry)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.DestinationCountry == destinationCountry;
            }).ToList();
        }

        public static List<BookingDTO> FilterByDepartureDate(List<BookingDTO> bookings,
            DateTime departureDate)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.DepartureDate == departureDate;
            }).ToList();
        }

        public static List<BookingDTO> FilterByDepartureAirport(List<BookingDTO> bookings,
            string departureAirport)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.DepartureAirport == departureAirport;
            }).ToList();
        }

        public static List<BookingDTO> FilterByArrivalAirport(List<BookingDTO> bookings,
            string arrivalAirport)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.ArrivalAirport == arrivalAirport;
            }).ToList();
        }

        public static List<BookingDTO> FilterByFlightClass(List<BookingDTO> bookings,
            FlightClass flightClass)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.Class == flightClass;
            }).ToList();
        }

        public static List<BookingDTO> FilterByFlightId(List<BookingDTO> bookings,
            int flightId)
        {
            return bookings.Where(booking =>
            {
                var flight = FlightService.GetById(booking.FlightId);
                return flight.Id == flightId;
            }).ToList();
        }

        public static List<BookingDTO> FilterByPassengerId(List<BookingDTO> bookings,
            int passengerId)
        {
            return bookings.Where(booking =>
            {
                var passenger = PassengerService.GetById(booking.PassengerId);
                return passenger.Id == passengerId;
            }).ToList();
        }

        public static bool Exists(int bookingId)
        {
            return GetAll().Any(booking => booking.Id == bookingId);
        }

        public static bool IsEmpty() => !GetAll().Any();
    }
}