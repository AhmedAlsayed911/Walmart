namespace Walmart.Application.Features.Order
{
    public static class OrderStatusFlow
    {
        public const int Pending = 0;
        public const int Processing = 1;
        public const int Shipped = 2;
        public const int Delivered = 3;
        public const int Cancelled = 4;

        public static bool IsValid(int status)
        {
            return status >= Pending && status <= Cancelled;
        }

        public static bool CanTransition(int currentStatus, int newStatus)
        {
            if (!IsValid(currentStatus) || !IsValid(newStatus))
                return false;

            if (currentStatus == newStatus)
                return true;

            return currentStatus switch
            {
                Pending => newStatus is Processing or Cancelled,
                Processing => newStatus is Shipped or Cancelled,
                Shipped => newStatus == Delivered,
                Delivered => false,
                Cancelled => false,
                _ => false
            };
        }
    }
}