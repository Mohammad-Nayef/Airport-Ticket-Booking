A .NET console application for airport ticket booking, The application should allow passengers to book tickets for flights and allow the manager to manage the bookings.

**For the Passenger**

1. The Passenger should be able to book a flight.
2. The passenger should be able to choose the class on a flight (Economy, Business, First Class), and the price for the flights varies according to the class.  
3. The passenger should be able to search for the **available** flight for booking by using one or more of the following parameters:
    - Price
    - Departure Country
    - Destination Country
    - Departure Date
    - Departure Airport
    - Arrival Airport
    - Class
4. The passenger should be able to cancel the booking.
5. The passenger should be able to modify the booking.
6. The passenger should be able to view his bookings.

**For the Manager**

1. The manager should have the ability to filter the bookings using one or more of the following parameters:
    - Flight
    - Price
    - Departure Country
    - Destination Country
    - Departure Date
    - Departure Airport
    - Arrival Airport
    - Passenger
    - Class
2. The user (the manager) should be able to add the list of the flights to the system at once, by importing a CSV file.
    1. Model level validations should be applied to the file to make sure the data are valid and not broken.
        1. Follow the common sense to choose the validations that should be applied to each field that makes the flight’s data valid for a real world scenarios.
    2. The returned list of errors should provide enough details for the manager to allocate and fix the error in the imported file.
    3. The manager should be able to have a **dynamically depending on model validations** to get the details about the type, constraints that should be applied on each field.
        
        *For Example: the returned result would be like:*
        
        - *Departure Country:*
            - *Free Text*
            - *Required*
        - *Departure Date*
            - *Date Time*
            - *Required*
            - *Allowed Range: today → future*
