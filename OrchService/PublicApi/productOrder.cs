namespace OrchService.PublicApi
{
    /// <summary>
    /// Индивидуальный "заказ" на продукт для конкретного пользователя.
    /// </summary>
    public class productOrder
    {
        public int UserId { get; set; }

        /// <summary>
        /// Наименование продукта. Должно быть уникальным.
        /// </summary>
        public required string ProductCaption { get; set; }
    }
}
