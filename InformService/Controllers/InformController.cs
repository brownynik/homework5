using Microsoft.AspNetCore.Mvc;

namespace InformService.Controllers
{
    /// <summary>
    /// Простое уведомление (лог, который можно развить в почтовые уведомления или рассылку в tg канал.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class InformController : Controller
    {
        private readonly ILogger<InformController> _log;

        /// <summary>
        /// Конструктор контроллера уведомления (рассылки).
        /// </summary>
        /// <param name="log">Лог события.</param>
        /// <exception cref="ArgumentNullException">Проверка на null аргумент.</exception>
        public InformController(ILogger<InformController> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        /// Приём сообщения для рассылки.
        /// </summary>
        /// <param name="msg">Информационный пакет с данными о продукте и покупателе.</param>
        [HttpPost(Name = "msg")]
        public void publiateInformation(InformationMessage msg)
        {
            _log.LogInformation($"{msg.eventDate}: продукт {msg.ProductName} заказан пользователем {msg.UserId}.");
        }
    }
}
