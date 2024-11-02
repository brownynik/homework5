namespace ShopMonitorService.ProductProcessingData
{
    /// <summary>
    /// Именованная процессинговая очередь обработки продукта.
    /// </summary>
    public class ProductProcessing
    {
        /// <summary>
        /// Наименование продукта, на который запущен этот процессинг.
        /// </summary>
        public required string ProductCaption { get; set; }

        /// <summary>
        /// Перечень пользователей (покупателей), ожидающих товар.
        /// </summary>
        public List<int>? UserIds { get; set; }

    }
}
