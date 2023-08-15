using AirportTicketBooking.Models;

namespace AirportTicketBooking.CSVFiles
{
    public static class PassengersFile
    {
        private const string PassengersFilePath = "Passengers.csv";

        public static bool HasData
        {
            get
            {
                return File.Exists(PassengersFilePath);
            }
        }

        public static void Append(PassengerDTO newPassenger)
        {
            if (Exists(newPassenger.Id))
            {
                throw new Exception("This passenger already exists.");
            }

            File.AppendAllText(PassengersFilePath, $"{newPassenger.Id}, {newPassenger.Name}\n");
        }

        public static List<PassengerDTO> GetAll()
        {
            var passengersList = new List<PassengerDTO>();
            var _fileReader = new StreamReader(PassengersFilePath);

            while (!_fileReader.EndOfStream)
            {
                var passengerData = _fileReader.ReadLine()?.Split(", ");

                // Passengers CSV file format: ID, Name
                passengersList.Add(new PassengerDTO(int.Parse(passengerData[0]),
                                                 passengerData[1]));
            }

            _fileReader.Close();
            return passengersList;
        }

        public static PassengerDTO Get(int passengerId)
        {
            if (!Exists(passengerId))
            {
                throw new Exception($"The passenger of ID: {passengerId} doesn't exist.");
            }

            return GetAll().Single(passenger => passenger.Id == passengerId);
        }

        public static void RemoveAll()
        {
            if (HasData)
                File.Delete(PassengersFilePath);
        }

        public static bool Exists(int passengerId)
        {
            if (HasData)
                return GetAll().Any(passenger => passenger.Id == passengerId);

            return false;
        }
    }
}