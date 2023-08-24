using System.ComponentModel.DataAnnotations;

namespace AirportTicketBooking
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var usedDate = (DateTime?)value ;

            if (usedDate?.Date < DateTime.Now.Date)
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The date of {name} must be in the future.";
        }
    }
}