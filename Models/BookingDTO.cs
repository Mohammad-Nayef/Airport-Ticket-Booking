using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class BookingDTO
    {
        private static int _nextUniqueId = 1;
        private static int NextUniqueId
        {
            get
            {
                if (!BookingService.IsEmpty())
                    return _nextUniqueId = BookingService.GetAll().Last().Id + 1;

                return _nextUniqueId;
            }
            set
            {
                _nextUniqueId = value;
            }
        }

        public int Id { get; private set; }

        public int PassengerId { get; private set; }

        public int FlightId { get; private set; }

        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Creates and stores the booking in bookings file.
        /// </summary>
        public BookingDTO(PassengerDTO passenger, FlightDTO flight)
        {
            PassengerId = passenger.Id;
            FlightId = flight.Id;
            BookingDate = DateTime.Now.Date;

            if (!BookingService.IsEmpty())
                NextUniqueId = BookingService.GetAll().Last().Id + 1;

            Id = NextUniqueId;
            BookingService.Add(this);
            NextUniqueId++;
        }

        public BookingDTO(int id, int passengerId, int flightId, DateTime bookingDate)
        {
            Id = id;
            PassengerId = passengerId;
            FlightId = flightId;
            BookingDate = bookingDate;
        }

        public override string ToString() => $"Booking -> ID: {Id}, Date: {BookingDate.Month}-" +
                $"{BookingDate.Day}-{BookingDate.Year}, Passenger and flight:\n" +
            $"{PassengerService.GetById(PassengerId)}\n{FlightService.GetById(FlightId)}";
    }
}