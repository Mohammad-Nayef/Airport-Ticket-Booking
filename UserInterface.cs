using System.Text;
using AirportTicketBooking.CSVFiles;
using AirportTicketBooking.Models;

namespace AirportTicketBooking
{
    public class UserInterface
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

                        if (String.IsNullOrWhiteSpace(passengerName))
                        {
                            Console.WriteLine("Enter a valid name.");
                            break;
                        }

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
                        availableFlights = FlightService.GetAllAvailable();

                        if (availableFlights.Count == 0)
                        {
                            Console.WriteLine("There are no available flights.");
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
                            passengerBookings = BookingService.GetBookingsOf(passenger.Id);

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
                passengerBookings = BookingService.GetBookingsOf(passenger.Id);

                if (passengerBookings.Count == 0) { 
                    Console.WriteLine("You have no bookings.");
                    break;
                }

                Console.Clear();
                Console.WriteLine("Booking Modify Menu\n");
                passengerBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                Console.WriteLine("0. Go back.");
                Console.Write("\nChoose an option or an ID of a booking to modify it: ");

                if (!int.TryParse(Console.ReadLine(), out var optionOrID) ||
                    !BookingService.GetBookingsOf(passenger.Id).Any(flight => flight.Id == optionOrID))
                {
                    if (optionOrID == 0)
                        return;
                    Console.WriteLine("Invalid ID.");
                }
                else
                {
                    availableFlights = FlightService.GetAllAvailable();
                    var modified = BookingAvailableFlightMenu();

                    if (modified)
                    {
                        BookingService.RemoveById(optionOrID);
                        Console.WriteLine($"The booking with ID ({optionOrID}) is modified successfully!");
                    }
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void CancelBookingMenu()
        {
            while (true)
            {
                passengerBookings = BookingService.GetBookingsOf(passenger.Id);

                if (passengerBookings.Count == 0)
                {
                    Console.WriteLine("You have no bookings.");
                    break;
                }

                Console.Clear();
                Console.WriteLine("Booking Cancel Menu\n");
                passengerBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                Console.WriteLine("0. Go back.");
                Console.Write("\nChoose an option or an ID of a booking to cancel it: ");

                if (!int.TryParse(Console.ReadLine(), out var optionOrID) ||
                    !BookingService.GetBookingsOf(passenger.Id).Any(flight => flight.Id == optionOrID))
                {
                    if (optionOrID == 0)
                        return;
                    Console.WriteLine("Invalid ID.");
                }
                else
                {
                    BookingService.RemoveById(optionOrID);
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
                Console.WriteLine("3. Import flights data as a CSV file (overwrite the existing file).");
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
                        Console.WriteLine(FlightValidator.GetConstraints());
                        break;

                    case '3':
                        Console.Write("Write the path of the file: ");
                        var filePath = Console.ReadLine();

                        try
                        {
                            FlightService.ImportCSVFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }

                        Console.WriteLine("File imported successfully!");
                        break;

                    case '4':
                        FlightService.RemoveAll();
                        BookingService.RemoveAll();
                        PassengerService.RemoveAll();
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
            string input = "";
            var chosen = new Dictionary<short, bool>();
            var bookings = new List<BookingDTO>();
            bookings = BookingService.GetAll().ToList();

            if (!bookings.Any())
            {
                Console.WriteLine("There are no bookings.");
                return;
            }

            var filteredBookings = bookings;

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
                        filteredBookings.ForEach(booking => Console.WriteLine(booking + "\n"));
                        break;

                    case 'b':
                        goto FilteringStart;

                    case '1':
                        if (!ValidFilter(ref chosen, ref input, "price", 1))
                            break;

                        if (!decimal.TryParse(input, out var price) ||
                            !bookings.Any(booking => FlightService.GetById(booking.FlightId).Price == price))
                        {
                            Console.WriteLine("Enter a valid and available price.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByPrice(filteredBookings, price);
                            menu.AppendLine($"Price: {price}");
                            chosen.Add(1, true);
                        }
                        break;

                    case '2':
                        if (!ValidFilter(ref chosen, ref input, "departure country", 2))
                            break;

                        if (!bookings.Any(booking =>
                            FlightService.GetById(booking.FlightId).DepartureCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByDepartureCountry(filteredBookings, input);
                            menu.AppendLine($"Departure country: {input}");
                            chosen.Add(2, true);
                        }
                        break;

                    case '3':
                        if (!ValidFilter(ref chosen, ref input, "destination country", 3))
                            break;

                        if (!bookings.Any(booking =>
                            FlightService.GetById(booking.FlightId).DestinationCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByDestinationCountry(filteredBookings, input);
                            menu.AppendLine($"Destination country: {input}");
                            chosen.Add(3, true);
                        }
                        break;

                    case '4':
                        if (!ValidFilter(ref chosen, ref input, "departure date (MM-DD-YYYY)", 4))
                            break;

                        if (!DateTime.TryParse(input, out var departureDate) ||
                            !bookings.Any(booking => FlightService.GetById(booking.FlightId).DepartureDate == departureDate))
                        {
                            Console.WriteLine("Enter a valid and available departure date.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByDepartureDate(filteredBookings, departureDate);
                            menu.AppendLine($"Departure date: {departureDate.Month}-{departureDate.Day}-{departureDate.Year}");
                            chosen.Add(4, true);
                        }
                        break;

                    case '5':
                        if (!ValidFilter(ref chosen, ref input, "departure airport", 5))
                            break;

                        if (!bookings.Any(booking =>
                            FlightService.GetById(booking.FlightId).DepartureAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available departure airport name.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByDepartureAirport(filteredBookings, input);
                            menu.AppendLine($"Departure airport: {input}");
                            chosen.Add(5, true);
                        }
                        break;

                    case '6':
                        if (!ValidFilter(ref chosen, ref input, "arrival airport", 6))
                            break;

                        if (!bookings.Any(booking =>
                            FlightService.GetById(booking.FlightId).ArrivalAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available arrival airport name.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByArrivalAirport(filteredBookings, input);
                            menu.AppendLine($"Arrival airport: {input}");
                            chosen.Add(6, true);
                        }
                        break;

                    case '7':
                        if (!ValidFilter(ref chosen, ref input, "flight class", 7))
                            break;

                        if (!Enum.TryParse(input, out FlightClass flightClass) ||
                            !bookings.Any(booking => FlightService.GetById(booking.FlightId).Class == flightClass))
                        {
                            Console.WriteLine("Enter a valid and available flight class.");
                        }
                        else
                        {
                            filteredBookings = BookingService.FilterByFlightClass(filteredBookings, flightClass);
                            menu.AppendLine($"Class: {flightClass}");
                            chosen.Add(7, true);
                        }
                        break;

                    case '8':
                        if (!ValidFilter(ref chosen, ref input, "flight ID", 8, false))
                            break;

                        FilterByFlightMenu(ref filteredBookings, ref menu, ref chosen);
                        break;

                    case '9':
                        if (!ValidFilter(ref chosen, ref input, "passenger ID", 9, false))
                            break;

                        FilterByPassengerMenu(ref filteredBookings, ref menu, ref chosen);
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

        public static void FilterByPassengerMenu(ref List<BookingDTO>? filteredBookings,
            ref StringBuilder menu, ref Dictionary<short, bool> chosen)
        {
            var passengers = PassengerService.GetAll();
            var passengersWithBookings = filteredBookings.Join(passengers,
                    booking => booking.PassengerId,
                    passenger => passenger.Id,
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

                if (!int.TryParse(input, out var passengerId) ||
                    !passengersWithBookings.Any(passenger => passenger.Id == passengerId))
                {
                    Console.WriteLine("Choose a valid and available passenger ID.");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    filteredBookings = BookingService.FilterByPassengerId(filteredBookings, passengerId);
                    menu.AppendLine($"Passenger ID: {passengerId}");
                    chosen.Add(9, true);
                    break;
                }
            }
        }

        public static void FilterByFlightMenu(ref List<BookingDTO>? filteredBookings,
            ref StringBuilder menu, ref Dictionary<short, bool> chosen)
        {
            var flights = FlightService.GetAll();
            var bookedFlights = filteredBookings.Join(flights,
                                booking => booking.FlightId,
                                flight => flight.Id,
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

                if (!int.TryParse(input, out var flightId) ||
                    !bookedFlights.Any(flight => flight.Id == flightId))
                {
                    Console.WriteLine("Choose a valid and available booked flight ID.");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    BookingService.FilterByFlightId(filteredBookings, flightId);
                    menu.AppendLine($"Flight ID: {flightId}");
                    chosen.Add(8, true);
                    break;
                }
            }
        }

        public static bool BookingAvailableFlightMenu()
        {
            var booked = false;

            while (true)
            {
                availableFlights = FlightService.GetAllAvailable();

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
                        booked = BookById() || booked;
                        break;

                    case 'b':
                        booked = FlightsFilterMenu() || booked;
                        break;

                    case '0':
                        return booked;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static bool BookById()
        {
            var booked = false;
            Console.WriteLine("0. Go back.\n");
            Console.Write("Enter an option or the ID of a flight to book it: ");

            if (!int.TryParse(Console.ReadLine(), out var ID) ||
                !availableFlights.Any(flight => flight.Id == ID))
            {
                if (ID == 0)
                    return booked;
                Console.WriteLine("Invalid ID.");
            }
            else
            {
                booked = true;
                var neededFlight = availableFlights.Single(flight => flight.Id == ID);
                var _ = new BookingDTO(passenger, neededFlight);

                Console.WriteLine($"The flight with ID ({ID}) is booked successfully!");
            }

            return booked;
        }

        public static bool FlightsFilterMenu()
        {
            var booked = false;
        FilteringStart:
            string input = "";
            var filteredAvailableFlights = availableFlights;
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

                        if (!decimal.TryParse(input, out var price) ||
                            !availableFlights.Any(flight => flight.Price == price))
                        {
                            Console.WriteLine("Enter a valid and available price.");
                        }
                        else
                        {
                            filteredAvailableFlights = FlightService.FilterByPrice(filteredAvailableFlights, price);
                            menu.AppendLine($"Price: {price}");
                            chosen.Add(1, true);
                        }
                        break;

                    case '2':
                        if (!ValidFilter(ref chosen, ref input, "departure country", 2))
                            break;

                        if (!availableFlights.Any(flight => flight.DepartureCountry
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available country name.");
                        }
                        else
                        {
                            filteredAvailableFlights = FlightService.FilterByDepartureCountry(filteredAvailableFlights, input);
                            menu.AppendLine($"Departure country: {input}");
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
                            filteredAvailableFlights = FlightService.FilterByDestinationCountry(filteredAvailableFlights, input);
                            menu.AppendLine($"Destination country: {input}");
                            chosen.Add(3, true);
                        }
                        break;

                    case '4':
                        if (!ValidFilter(ref chosen, ref input, "departure date (MM-DD-YYYY)", 4))
                            break;

                        if (!DateTime.TryParse(input, out var departureDate) ||
                            !availableFlights.Any(flight => flight.DepartureDate == departureDate))
                        {
                            Console.WriteLine("Enter a valid and available departure date.");
                        }
                        else
                        {
                            filteredAvailableFlights = FlightService.FilterByDepartureDate(filteredAvailableFlights, departureDate);
                            menu.AppendLine($"Departure date: {departureDate.Month}-{departureDate.Day}-{departureDate.Year}");
                            chosen.Add(4, true);
                        }
                        break;

                    case '5':
                        if (!ValidFilter(ref chosen, ref input, "departure airport", 5))
                            break;

                        if (!availableFlights.Any(flight => flight.DepartureAirport
                            .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Console.WriteLine("Enter a valid and available departure airport name.");
                        }
                        else
                        {
                            filteredAvailableFlights = FlightService.FilterByDepartureAirport(filteredAvailableFlights, input);
                            menu.AppendLine($"Departure airport: {input}");
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
                            filteredAvailableFlights = FlightService.FilterByArrivalAirport(filteredAvailableFlights, input);
                            menu.AppendLine($"Arrival airport: {input}");
                            chosen.Add(6, true);
                        }
                        break;

                    case '7':
                        if (!ValidFilter(ref chosen, ref input, "flight class", 7))
                            break;

                        if (!Enum.TryParse(input, out FlightClass flightClass) ||
                            !availableFlights.Any(flight => flight.Class == flightClass))
                        {
                            Console.WriteLine("Enter a valid and available flight class.");
                        }
                        else
                        {
                            filteredAvailableFlights = FlightService.FilterByClass(filteredAvailableFlights, flightClass);
                            menu.AppendLine($"Class: {flightClass}");
                            chosen.Add(7, true);
                        }
                        break;

                    case '0':
                        return booked;

                    case 'a':
                        Console.Clear();
                        filteredAvailableFlights.ForEach(flight => Console.WriteLine(flight + "\n"));
                        booked = BookById();
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