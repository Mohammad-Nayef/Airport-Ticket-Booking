﻿using AirportTicketBooking.Models;

namespace AirportTicketBooking.Repository
{
    public static class BookingsRepository
    {
        private const string BookingsFilePath = "Bookings.csv";
        private static bool _loaded = false;
        private static List<BookingDTO> _bookings = new();

        public static List<BookingDTO> Bookings
        {
            get
            {
                if (!_loaded && File.Exists(BookingsFilePath))
                {
                    _loaded = true;
                    _bookings = LoadFromFile(BookingsFilePath);
                }

                return _bookings;
            }
        }

        public static List<BookingDTO> LoadFromFile(string bookingsFilePath)
        {
            var bookings = new List<BookingDTO>();
            var fileReader = new StreamReader(bookingsFilePath);

            while (!fileReader.EndOfStream)
            {
                var bookingData = fileReader.ReadLine()?.Split(", ");
                var bookingId = int.Parse(bookingData[0]);
                var passengerId = int.Parse(bookingData[1]);
                var flightId = int.Parse(bookingData[2]);

                bookings.Add(new BookingDTO(bookingId, passengerId, flightId,
                    DateTime.Parse(bookingData[3])));
            }

            fileReader.Close();
            return bookings;
        }

        public static void Add(BookingDTO newBooking)
        {
            _bookings.Add(newBooking);
            StoreInFile(newBooking);
        }

        public static void StoreInFile(BookingDTO newBooking)
        {
            _loaded = true;
            File.AppendAllText(BookingsFilePath, $"{newBooking.Id}, {newBooking.PassengerId}," +
                       $" {newBooking.FlightId}, {newBooking.BookingDate.Month}-" +
                       $"{newBooking.BookingDate.Day}-{newBooking.BookingDate.Year}\n");
        }

        public static void Overwrite(List<BookingDTO> newBookings)
        {
            Delete();
            newBookings.ForEach(booking => Add(booking));
        }

        public static void Delete()
        {
            if (File.Exists(BookingsFilePath))
                File.Delete(BookingsFilePath);

            _bookings.Clear();
        }
    }
}