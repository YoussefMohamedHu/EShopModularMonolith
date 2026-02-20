namespace order.Orders.ValueObjects
{
    public record Payment
    {
        public string CardName { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpirationDate { get; private set; }
        public string CVV { get; private set; }

        private Payment() { }
        private Payment(string cardName, string cardNumber, string expirationDate, string cvv)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            ExpirationDate = expirationDate;
            CVV = cvv;
        }

        public static Payment Of(string cardName, string cardNumber, string expirationDate, string cvv)
        {
            return new Payment(cardName, cardNumber, expirationDate, cvv);
        }
    }
}
