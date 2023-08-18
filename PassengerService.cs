using AirportTicketBooking.Models;
using AirportTicketBooking.Repository;

namespace AirportTicketBooking.CSVFiles
{
    public static class PassengerService
    {
        public static List<PassengerDTO> GetAll() => PassengersRepository.Passengers;

        public static PassengerDTO GetById(int passengerId)
        {
            if (!Exists(passengerId))
            {
                throw new Exception($"The passenger of ID {passengerId} doesn't exist.");
            }

            return GetAll().Single(passenger => passenger.Id == passengerId);
        }

        public static void Add(PassengerDTO newPassenger)
        {
            if (Exists(newPassenger.Id))
            {
                throw new Exception($"The passenger {newPassenger.Name} already exists.");
            }

            PassengersRepository.Add(newPassenger);
        }

        public static void RemoveAll() => PassengersRepository.Delete();

        public static bool Exists(int passengerId)
        {
            return GetAll().Any(passenger => passenger.Id == passengerId);
        }

        public static bool IsEmpty() => !GetAll().Any();
    }
}