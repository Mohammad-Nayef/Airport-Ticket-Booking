using AirportTicketBooking.Models;

namespace AirportTicketBooking.CSVFiles
{
    public static class BookingService
    {
        private const string BookingsFilePath = "Bookings.csv";

        public static bool HasData
        {
            get
            {
                var fileInfo = new FileInfo(BookingsFilePath);
                return fileInfo.Exists && fileInfo.Length > 0;
            }
        }

        public static void Append(BookingDTO newBooking)
        {
            if (Exists(newBooking.Id))
            {
                throw new Exception("This booking already exists.");
            }

            File.AppendAllText(BookingsFilePath, $"{newBooking.Id}, {newBooking.PassengerId}," +
                                   $" {newBooking.FlightId}, {newBooking.BookingDate.Month}-" +
                                   $"{newBooking.BookingDate.Day}-{newBooking.BookingDate.Year}\n");
        }

        public static void Overwrite(List<BookingDTO> newBookings)
        {
            if (HasData) 
                File.Delete(BookingsFilePath);

            newBookings.ForEach(booking => Append(booking));
        }

        public static List<BookingDTO> GetAll()
        {
            if (!HasData)
            {
                throw new Exception("There are no bookings.");
            }

            var bookingsList = new List<BookingDTO>();
            var _fileReader = new StreamReader(BookingsFilePath);

            while (!_fileReader.EndOfStream)
            {
                var bookingData = _fileReader.ReadLine()?.Split(", ");
                int passengerId = int.Parse(bookingData[1]);
                int flightId = int.Parse(bookingData[2]);

                // Bookings CSV file format: ID, Passenger ID, Flight ID, Booking date
                bookingsList.Add(new BookingDTO(int.Parse(bookingData[0]), 
                    PassengerService.Get(passengerId), FlightService.Get(flightId),
                    DateTime.Parse(bookingData[3])));
            }

            _fileReader.Close();
            return bookingsList;
        }

        public static List<BookingDTO> GetBookingsOf(int passengerId)
        {
            if (!HasData)
                throw new Exception("There are no bookings.");
            return GetAll().Where(booking => booking.PassengerId == passengerId)
                            .ToList();
        }

        public static void Remove(int bookingId)
        {
            if (!Exists(bookingId))
            {
                throw new Exception("This bookings doesn't exist.");
            }

            var modifiedBookings = GetAll().Where(booking => booking.Id != bookingId).ToList();
            Overwrite(modifiedBookings);
        }

        public static void RemoveAll()
        {
            if (HasData)
                File.Delete(BookingsFilePath);
        }

        public static bool Exists(int bookingId)
        {
            if (HasData)
                return GetAll().Any(booking => booking.Id == bookingId);

            return false;
        }
    }
}