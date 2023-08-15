using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking.Models
{
    public class BookingDTO
    {
        private static int NextUniqueId { get; set; } = 1;

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

            if (BookingService.HasData)
            {
                NextUniqueId = BookingService.GetAll().Last().Id + 1;
            }

            Id = NextUniqueId;
            BookingService.Append(this);
            NextUniqueId++;
        }

        public BookingDTO(int id, PassengerDTO passenger, FlightDTO flight, DateTime bookingDate)
        {
            Id = id;
            PassengerId = passenger.Id;
            FlightId = flight.Id;
            BookingDate = bookingDate;
        }

        public override string ToString() => $"Booking -> ID: {Id}, Date: {BookingDate.Month}-" +
                $"{BookingDate.Day}-{BookingDate.Year}, Passenger and flight:\n{PassengerId}\n{FlightId}";
    }
}