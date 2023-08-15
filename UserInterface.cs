using System.Text;
using AirportTicketBooking.CSVFiles;

namespace AirportTicketBooking
{
    public class UI
    {
        private static char? _option;
        private static PassengerDTO passenger;
        private static List<FlightDTO> availableFlights = new();
        private static List<BookingDTO> passengerBookings = new();

        public static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Airport Ticket Booking\n");
                Console.WriteLine("1. Passenger.");
                Console.WriteLine("2. Manager.");
                Console.WriteLine("0. Exit the program.");
                Console.Write("\nChoose your role: ");
                _option = Console.ReadLine()?.First();

                switch (_option)
                {
                    case '1':
                        string passengerName;
                        Console.Write("Please write your full name: ");
                        passengerName = Console.ReadLine();
                        passenger = new PassengerDTO(passengerName);

                        PassengerMenu();
                        break;

                    case '2':
                        ManagerMenu();
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void PassengerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Passenger Menu\n");
                Console.WriteLine("1. Book a flight from the available flights.");
                Console.WriteLine("2. Modify a booking.");
                Console.WriteLine("3. View my bookings.");
                Console.WriteLine("4. Cancel a booking.");
                Console.WriteLine("0. Go back.");
                Console.Write("\n Choose an option: ");
                _option = Console.ReadLine()?.First();

                switch (_option)
                {
                    case '1':
                        try
                        {
                            availableFlights = FlightsFile.GetAllAvailable();

                            if (availableFlights.Count == 0)
                                throw new Exception("There are no available flights.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }

                        BookingAvailableFlightMenu();
                        break;

                    case '2':
                        ModifyBookingMenu();
                        break;

                    case '3':
                        try
                        {
                            passengerBookings = BookingsFile.GetBookingsOf(passenger.ID);

                            if (passengerBookings.Count == 0)
                                throw new Exception("You have no bookings");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }

                        passengerBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                        break;

                    case '4':
                        CancelBookingMenu();
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        public static void ModifyBookingMenu()
        {
            while (true)
            {
                try
                {
                    passengerBookings = BookingsFile.GetBookingsOf(passenger.ID);

                    if (passengerBookings.Count == 0)
                        throw new Exception("You have no bookings.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }

                Console.Clear();
                Console.WriteLine("Booking Modify Menu\n");
                passengerBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                Console.WriteLine("0. Go back.");
                Console.Write("\nChoose an option or an ID of a booking to modify it: ");

                if (!int.TryParse(Console.ReadLine(), out var optionOrID) ||
                    !BookingsFile.GetBookingsOf(passenger.ID).Any(flight => flight.ID == optionOrID))
                {
                    if (optionOrID == 0)
                        return;
                    Console.WriteLine("Invalid ID.");
                }
                else
                {
                    availableFlights = FlightsFile.GetAllAvailable();
                    BookingAvailableFlightMenu();
                    BookingsFile.Remove(optionOrID);
                    Console.WriteLine($"The booking with ID ({optionOrID}) is modified successfully!");
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void CancelBookingMenu()
        {
            while (true)
            {
                try
                {
                    passengerBookings = BookingsFile.GetBookingsOf(passenger.ID);

                    if (passengerBookings.Count == 0)
                        throw new Exception("You have no bookings.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }

                Console.Clear();
                Console.WriteLine("Booking Cancel Menu\n");
                passengerBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                Console.WriteLine("0. Go back.");
                Console.Write("\nChoose an option or an ID of a booking to cancel it: ");

                if (!int.TryParse(Console.ReadLine(), out var optionOrID) ||
                    !BookingsFile.GetBookingsOf(passenger.ID).Any(flight => flight.ID == optionOrID))
                {
                    if (optionOrID == 0)
                        return;
                    Console.WriteLine("Invalid ID.");
                }
                else
                {
                    BookingsFile.Remove(optionOrID);
                    Console.WriteLine($"The booking with ID ({optionOrID}) is canceled successfully!");
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void ManagerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Manager Menu\n");
                Console.WriteLine("1. Filter the bookings.");
                Console.WriteLine("2. View the flights CSV file constraints to avoid errors.");
                Console.WriteLine("3. Import flights data as a CSV file.");
                Console.WriteLine("4. Remove all of the stored data.");
                Console.WriteLine("0. Go back.");
                Console.Write("\nChoose an option: ");
                _option = Console.ReadLine()?.First();

                switch (_option)
                {
                    case '1':
                        FilterBookingsMenu();
                        break;

                    case '2':
                        Console.Clear();
                        Console.WriteLine("Flights CSV file constraints:\n");
                        Console.WriteLine(FlightDTO.GetConstraints());
                        break;

                    case '3':
                        Console.Write("Write the path of the file: ");
                        var filePath = Console.ReadLine();

                        try
                        {
                            FlightsFile.ImportCSVFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }

                        Console.WriteLine("File imported successfully!");
                        break;

                    case '4':
                        FlightsFile.RemoveAll();
                        BookingsFile.RemoveAll();
                        PassengersFile.RemoveAll();
                        Console.WriteLine("Data deleted successfully!");
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void FilterBookingsMenu()
        {
        FilteringStart:
            decimal price = 0;
            int flightID = 0, passengerID = 0;
            string departureCountry = "", destinationCountry = "", departureAirport = "",
                arrivalAirport = "", input = "";
            var departureDate = new DateTime();
            var flightClass = new FlightClass();
            var chosen = new Dictionary<short, bool>();
            var bookings = new List<BookingDTO>();

            try
            {
                bookings = BookingsFile.GetAll().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var menu = new StringBuilder("Filter The Bookings\n\n" +
                                         "a. Print the results.\n" +
                                         "b. Cancel the filters.\n" +
                                         "\nFilter by (Multiple options are allowed):\n" +
                                         "1. Price.\n" +
                                         "2. Departure country.\n" +
                                         "3. Destination Country.\n" +
                                         "4. Departure Date.\n" +
                                         "5. Departure Airport.\n" +
                                         "6. Arrival Airport.\n" +
                                         "7. Class.\n" +
                                         "8. Flight.\n" +
                                         "9. Passenger.\n" +
                                         "0. Go back.\n\n");

            while (true)
            {
                Console.Clear();
                bookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                Console.WriteLine(menu.ToString());
                Console.Write("\nChoose an option: ");
                _option = Console.ReadLine()?.ToLower().First();

                switch (_option)
                {
                    case 'a':
                        Console.Clear();
                        var filteredBookings = bookings;

                        if (chosen.ContainsKey(1))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.Price == price)
                                .ToList();
                        if (chosen.ContainsKey(2))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.DepartureCountry
                                .Equals(departureCountry, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(3))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.DestinationCountry
                                .Equals(destinationCountry, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(4))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.DepartureDate == departureDate)
                                .ToList();
                        if (chosen.ContainsKey(5))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.DepartureAirport
                                .Equals(departureAirport, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(6))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.ArrivalAirport
                                .Equals(arrivalAirport, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(7))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.Class == flightClass)
                                .ToList();
                        if (chosen.ContainsKey(8))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Flight.ID == flightID)
                                .ToList();
                        if (chosen.ContainsKey(9))
                            filteredBookings = filteredBookings
                                .Where(booking => booking.Passenger.ID == passengerID)
                                .ToList();

                        filteredBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                        break;

                    case 'b':
                        goto FilteringStart;

                    case '1':
                        if (!ValidFilter(ref chosen, ref input, "price", 1))
                            break;

                        if (!decimal.TryParse(input, out price) ||
                            !bookings.Any(booking => booking.Flight.Price == price))
                        {
                            Console.WriteLine("Enter a valid and available price.");
                        }
                        else
                        {
                            menu.AppendLine($"Price: {price}");
                            chosen.Add(1, true);
                        }
                        break;

                    case '2':
                        if (!ValidFilter(ref chosen, ref input, "departure country", 2))
                            break;

                        if (!bookings.Any(booking =>
                            booking.Flight.DepartureCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            departureCountry = input;
                            menu.AppendLine($"Departure country: {departureCountry}");
                            chosen.Add(2, true);
                        }
                        break;

                    case '3':
                        if (!ValidFilter(ref chosen, ref input, "destination country", 3))
                            break;

                        if (!bookings.Any(booking =>
                            booking.Flight.DestinationCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            destinationCountry = input;
                            menu.AppendLine($"Destination country: {destinationCountry}");
                            chosen.Add(3, true);
                        }
                        break;

                    case '4':
                        if (!ValidFilter(ref chosen, ref input, "departure date (MM-DD-YYYY)", 4))
                            break;

                        if (!DateTime.TryParse(input, out departureDate) ||
                            !bookings.Any(booking => booking.Flight.DepartureDate == departureDate))
                        {
                            Console.WriteLine("Enter a valid and available departure date.");
                        }
                        else
                        {
                            menu.AppendLine($"Departure date: {departureDate.Month}-" +
                                $"{departureDate.Day}-{departureDate.Year}");
                            chosen.Add(4, true);
                        }
                        break;

                    case '5':
                        if (!ValidFilter(ref chosen, ref input, "departure airport", 5))
                            break;

                        if (!bookings.Any(booking =>
                            booking.Flight.DepartureAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available departure airport name.");
                        }
                        else
                        {
                            departureAirport = input;
                            menu.AppendLine($"Departure airport: {departureAirport}");
                            chosen.Add(5, true);
                        }
                        break;

                    case '6':
                        if (!ValidFilter(ref chosen, ref input, "arrival airport", 6))
                            break;

                        if (!bookings.Any(booking =>
                            booking.Flight.ArrivalAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available arrival airport name.");
                        }
                        else
                        {
                            arrivalAirport = input;
                            menu.AppendLine($"Arrival airport: {arrivalAirport}");
                            chosen.Add(6, true);
                        }
                        break;

                    case '7':
                        if (!ValidFilter(ref chosen, ref input, "flight class", 7))
                            break;

                        if (!Enum.TryParse(input, out flightClass) ||
                            !bookings.Any(booking => booking.Flight.Class == flightClass))
                        {
                            Console.WriteLine("Enter a valid and available flight class.");
                        }
                        else
                        {
                            menu.AppendLine($"Class: {flightClass}");
                            chosen.Add(7, true);
                        }
                        break;

                    case '8':
                        if (!ValidFilter(ref chosen, ref input, "flight ID", 8, false))
                            break;

                        FilterByFlightMenu(ref bookings, ref menu, ref chosen, ref flightID);
                        break;

                    case '9':
                        if (!ValidFilter(ref chosen, ref input, "passenger ID", 9, false))
                            break;

                        FilterByPassengerMenu(ref bookings, ref menu, ref chosen, ref passengerID);
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        public static void FilterByPassengerMenu(ref List<BookingDTO>? bookings,
            ref StringBuilder menu, ref Dictionary<short, bool> chosen, ref int passengerID)
        {
            var passengers = PassengersFile.GetAll();
            var passengersWithBookings = bookings.Join(passengers,
                    booking => booking.Passenger.ID,
                    passenger => passenger.ID,
                    (booking, passenger) => passenger)
                    .Distinct()
                    .ToList();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select an ID of a passenger to filter bookings by it:");
                passengersWithBookings.ForEach(passenger => Console.WriteLine(passenger + "\n"));
                Console.WriteLine("\n0. Go back.");
                Console.Write("Choose a valid ID: ");
                var input = Console.ReadLine();

                if (input == "0")
                    return;

                if (!int.TryParse(input, out var newPassengerID) ||
                    !passengersWithBookings.Any(passenger => passenger.ID == newPassengerID))
                {
                    Console.WriteLine("Choose a valid and available passenger ID.");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    passengerID = newPassengerID;
                    menu.AppendLine($"Passenger ID: {passengerID}");
                    chosen.Add(9, true);
                    break;
                }
            }
        }

        public static void FilterByFlightMenu(ref List<BookingDTO>? bookings,
            ref StringBuilder menu, ref Dictionary<short, bool> chosen, ref int flightID)
        {
            var flights = FlightsFile.GetAll();
            var bookedFlights = bookings.Join(flights,
                                booking => booking.Flight.ID,
                                flight => flight.ID,
                                (booking, flight) => flight)
                                .Distinct()
                                .ToList();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select an ID of a flight to filter bookings by it:");
                bookedFlights.ForEach(flight => Console.WriteLine(flight + "\n"));
                Console.WriteLine("0. Go back.");
                Console.Write("Choose a valid ID: ");
                var input = Console.ReadLine();

                if (input == "0")
                    return;

                if (!int.TryParse(input, out var newFlightID) ||
                    !bookedFlights.Any(flight => flight.ID == newFlightID))
                {
                    Console.WriteLine("Choose a valid and available booked flight ID.");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    flightID = newFlightID;
                    menu.AppendLine($"Flight ID: {flightID}");
                    chosen.Add(8, true);
                    break;
                }
            }
        }

        public static void BookingAvailableFlightMenu()
        {
            while (true)
            {
                availableFlights = FlightsFile.GetAllAvailable();

                Console.Clear();
                Console.WriteLine("All of the available flights:");
                availableFlights.ForEach(flight =>
                    Console.WriteLine(flight + "\n"));
                Console.WriteLine("a. Book a flight using its ID.");
                Console.WriteLine("b. Search for available flights " +
                    "using specific parameter(s).");
                Console.WriteLine("0. Go back.");
                Console.Write("\n Choose an option: ");
                _option = Console.ReadLine()?.First();

                switch (_option)
                {
                    case 'a':
                        BookByID();
                        break;

                    case 'b':
                        FlightsFilterMenu();
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void BookByID()
        {
            Console.WriteLine("0. Go back.\n");
            Console.Write("Enter an option or the ID of a flight to book it: ");

            if (!int.TryParse(Console.ReadLine(), out var ID) ||
                !availableFlights.Any(flight => flight.ID == ID))
            {
                if (ID == 0)
                    return;
                Console.WriteLine("Invalid ID.");
            }
            else
            {
                var neededFlight = availableFlights.Single(flight => flight.ID == ID);
                var _ = new BookingDTO(passenger, neededFlight);

                Console.WriteLine($"The flight with ID ({ID}) is booked successfully!");
            }
        }

        public static void FlightsFilterMenu()
        {
        FilteringStart:
            decimal price = 0;
            string departureCountry = "", destinationCountry = "", departureAirport = "",
                arrivalAirport = "", input = "";
            var departureDate = new DateTime();
            var flightClass = new FlightClass();
            var chosen = new Dictionary<short, bool>();

            var menu = new StringBuilder("Flights Search Menu\n\n" +
                                         "a. Print the result.\n" +
                                         "b. Cancel the filters.\n" +
                                         "\nSelect one or more of these parameters:\n" +
                                         "1. Price.\n" +
                                         "2. Departure country.\n" +
                                         "3. Destination Country.\n" +
                                         "4. Departure Date.\n" +
                                         "5. Departure Airport.\n" +
                                         "6. Arrival Airport.\n" +
                                         "7. Class.\n" +
                                         "0. Go back.\n");

            while (true)
            {
                Console.Clear();
                availableFlights.ForEach(flight => Console.WriteLine(flight + "\n"));
                Console.WriteLine(menu.ToString());
                Console.Write("\nChoose a valid option: ");
                _option = Console.ReadLine()?.First();

                switch (_option)
                {
                    case '1':
                        if (!ValidFilter(ref chosen, ref input, "price", 1))
                            break;

                        if (!decimal.TryParse(input, out price) ||
                            !availableFlights.Any(flight => flight.Price == price))
                        {
                            Console.WriteLine("Enter a valid and available price.");
                        }
                        else
                        {
                            menu.AppendLine($"Price: {price}");
                            chosen.Add(1, true);
                        }
                        break;

                    case '2':
                        if (!ValidFilter(ref chosen, ref input, "departure country", 2))
                            break;

                        if (!availableFlights.Any(flight =>
                            flight.DepartureCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            departureCountry = input;
                            menu.AppendLine($"Departure country: {departureCountry}");
                            chosen.Add(2, true);
                        }
                        break;

                    case '3':
                        if (!ValidFilter(ref chosen, ref input, "destination country", 3))
                            break;

                        if (!availableFlights.Any(flight =>
                            flight.DestinationCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            destinationCountry = input;
                            menu.AppendLine($"Destination country: {destinationCountry}");
                            chosen.Add(3, true);
                        }
                        break;

                    case '4':
                        if (!ValidFilter(ref chosen, ref input, "departure date (MM-DD-YYYY)", 4))
                            break;

                        if (!DateTime.TryParse(input, out departureDate) ||
                            !availableFlights.Any(flight => flight.DepartureDate == departureDate))
                        {
                            Console.WriteLine("Enter a valid and available departure date.");
                        }
                        else
                        {
                            menu.AppendLine($"Departure date: {departureDate.Month}-" +
                                $"{departureDate.Day}-{departureDate.Year}");
                            chosen.Add(4, true);
                        }
                        break;

                    case '5':
                        if (!ValidFilter(ref chosen, ref input, "departure airport", 5))
                            break;

                        if (!availableFlights.Any(flight =>
                            flight.DepartureAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available departure airport name.");
                        }
                        else
                        {
                            departureAirport = input;
                            menu.AppendLine($"Departure airport: {departureAirport}");
                            chosen.Add(5, true);
                        }
                        break;

                    case '6':
                        if (!ValidFilter(ref chosen, ref input, "arrival airport", 6))
                            break;

                        if (!availableFlights.Any(flight =>
                            flight.ArrivalAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available arrival airport name.");
                        }
                        else
                        {
                            arrivalAirport = input;
                            menu.AppendLine($"Arrival airport: {arrivalAirport}");
                            chosen.Add(6, true);
                        }
                        break;

                    case '7':
                        if (!ValidFilter(ref chosen, ref input, "flight class", 7))
                            break;

                        if (!Enum.TryParse(input, out flightClass) ||
                            !availableFlights.Any(flight => flight.Class == flightClass))
                        {
                            Console.WriteLine("Enter a valid and available flight class.");
                        }
                        else
                        {
                            menu.AppendLine($"Class: {flightClass}");
                            chosen.Add(7, true);
                        }
                        break;

                    case '0':
                        return;

                    case 'a':
                        Console.Clear();
                        var filteredAvailableFlights = availableFlights;

                        if (chosen.ContainsKey(1))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.Price == price)
                                .ToList();
                        if (chosen.ContainsKey(2))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(booking => booking.DepartureCountry
                                .Equals(departureCountry, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(3))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.DestinationCountry
                                .Equals(destinationCountry, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(4))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.DepartureDate == departureDate)
                                .ToList();
                        if (chosen.ContainsKey(5))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.DepartureAirport
                                .Equals(departureAirport, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(6))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.ArrivalAirport
                                .Equals(arrivalAirport, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();
                        if (chosen.ContainsKey(7))
                            filteredAvailableFlights = filteredAvailableFlights
                                .Where(flight => flight.Class == flightClass)
                                .ToList();

                        filteredAvailableFlights.ForEach(flight => Console.WriteLine(flight + "\n"));
                        BookByID();
                        break;

                    case 'b':
                        goto FilteringStart;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static bool ValidFilter(ref Dictionary<short, bool> chosen,
            ref string? input, string str, short index, bool useInput = true)
        {
            if (chosen.ContainsKey(index))
            {
                Console.WriteLine("Already chosen.");
                return false;
            }

            if (useInput)
            {
                Console.Write($"Enter the {str}: ");
                input = Console.ReadLine();
            }
            return true;
        }
    }
}