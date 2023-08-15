using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking
{
    public class BookingDTO
    {
        public static int NextUniqueID { get; private set; } = 1;

        public int ID { get; private set; }

        public PassengerDTO Passenger { get; private set; }

        public FlightDTO Flight { get; private set; }

        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Creates and stores the booking in bookings file.
        /// </summary>
        public BookingDTO(PassengerDTO passenger, FlightDTO flight)
        {
            Passenger = passenger;
            Flight = flight;
            BookingDate = DateTime.Now.Date;

            if (BookingsFile.HasData)
            {
                NextUniqueID = BookingsFile.GetAll().Last().ID + 1;
            }

            ID = NextUniqueID;
            BookingsFile.Append(this);
            NextUniqueID++;
        }

        public BookingDTO(int id, PassengerDTO passenger, FlightDTO flight, DateTime bookingDate)
        {
            ID = id;
            Passenger = passenger;
            Flight = flight;
            BookingDate = bookingDate;
        }

        public override string ToString() => $"Booking -> ID: {ID}, Date: {BookingDate.Month}-" +
                $"{BookingDate.Day}-{BookingDate.Year}, Passenger and flight:\n{Passenger}\n{Flight}";
    }
}