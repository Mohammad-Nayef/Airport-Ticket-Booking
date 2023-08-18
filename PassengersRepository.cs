using AirportTicketBooking.Models;

namespace AirportTicketBooking.Repository
{
    public static class PassengersRepository
    {
        private const string PassengersFilePath = "Passengers.csv";
        private static bool _loaded = false;
        private static List<PassengerDTO> _passengers = new();

        public static List<PassengerDTO> Passengers
        {
            get
            {
                if (!_loaded && File.Exists(PassengersFilePath))
                {
                    _loaded = true;
                    _passengers = LoadFromFile(PassengersFilePath);
                }

                return _passengers;
            }
        }

        private static List<PassengerDTO> LoadFromFile(string passengersFilePath)
        {
            var passengers = new List<PassengerDTO>();
            var _fileReader = new StreamReader(PassengersFilePath);

            while (!_fileReader.EndOfStream)
            {
                var passengerData = _fileReader.ReadLine()?.Split(", ");
                var id = int.Parse(passengerData[0]);
                var name = passengerData[1];
                
                passengers.Add(new PassengerDTO(int.Parse(passengerData[0]),
                                                 passengerData[1]));
            }

            _fileReader.Close();
            return passengers;
        }

        public static void Add(PassengerDTO newPassenger)
        {
            _passengers.Add(newPassenger);
            StoreInFile(newPassenger);
        }

        private static void StoreInFile(PassengerDTO newPassenger)
        {
            _loaded = true;
            File.AppendAllText(PassengersFilePath, $"{newPassenger.Id}, {newPassenger.Name}\n");
        }

        public static void Delete()
        {
            if (File.Exists(PassengersFilePath))
                File.Delete(PassengersFilePath);

            _passengers.Clear();
        }
    }
}