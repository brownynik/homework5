using System.ComponentModel.DataAnnotations;

namespace InformService
{
    /// <summary>
    /// Информационный контейнер для пересылки.
    /// </summary>
    public class InformationMessage
    {
        /// <summary>
        /// Идентификатор сообщения для агрегирования по каналам и/или обеспечения идемпотентности (проверка идемпотентности пока не реализована :) ).
        /// </summary>
        public Guid aggregateId {  get; set; }
        
        /// <summary>
        /// Дата уведомления.
        /// </summary>
        public DateTime eventDate { get; set; }

        /// <summary>
        /// Наименование продукта.
        /// </summary>
        public required string ProductName { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }
    }
}
