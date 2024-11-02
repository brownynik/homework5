using InformService;
using Microsoft.AspNetCore.Mvc;
using OrchService.ExternalServices;
using OrchService.PublicApi;
using ShopMonitorService.ProductProcessingData;

namespace OrchService.Controllers
{
    /// <summary>
    /// Оркестрация мониторнга доступных продуктов, уведомления о резервировании и запросы пользователей.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrchestrateController : ControllerBase
    {
        private readonly ILogger<OrchestrateController> _log;
        private List<ProductProcessing> _processing;
        private readonly InformServiceProvide _informSrv;
        private readonly ShopMonitorServiceProvide _monitorPrime;
        private readonly ShopMonitorServiceProvide _monitorSecundo;

        public OrchestrateController(ILogger<OrchestrateController> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _processing = new List<ProductProcessing>();

            // Обеспечиваем соединение с сервисом уведомления и сервисами мониторинга.
            // Да, в данной упрощённой модели - просто по http.
            _informSrv = InformServiceProvide.GetSingleSrv("http://localhost:5168/");
            
            // По идее, тут желательно сделать коллекцию и собирать её через zeroconf.
            _monitorPrime = ShopMonitorServiceProvide.GetSingleSrv("http://localhost:5090/");
            
            // Вот тут при создании контейнера или compose файла обязательно не забыть явно повесить на новый порт второй экземпляр сервиса.
            _monitorSecundo = ShopMonitorServiceProvide.GetSingleSrv("http://localhost:5091/");
        }

        /// <summary>
        /// Добавляем в процесс ожидания нового потребителя (ожидающий продукта пользователь).
        /// </summary>
        /// <param name="userOrder"></param>
        [HttpGet(Name = "reg-to-wait")]
        public void regOrderForWait(productOrder order)
        {
            var productProcessing = _processing.First(p => p.ProductCaption == order.ProductCaption);

            if (productProcessing == null)
            {
                // Создаём новый процессинг ожидания продукта.
                _processing.Add(new ProductProcessing
                {
                    ProductCaption = order.ProductCaption,
                    UserIds = [order.UserId],
                });
            }
            
            // Иначе просто добавляем ожадающего в именованный процессинг.
            else
            {
                if (productProcessing.UserIds == null)
                {
                    productProcessing.UserIds = new List<int>();
                }

                productProcessing.UserIds.Add(order.UserId);
            }
        }

        /// <summary>
        /// Событие реордеринга процессинга при обработке заказа.
        /// </summary>
        /// <param name="order"></param>
        [HttpGet(Name = "reserve-product")]
        public void ReserveProductToUser(productOrder order)
        {
            var productProcessing = _processing.First(p => p.ProductCaption == order.ProductCaption);

            if (productProcessing == null)
            {
                // По идее, тут можно запрашивать создание нового процессинга и мониторинга, но модель упрощена.
                _log.LogInformation($"{DateTime.Now}: не найдена очередь на продукт {order.ProductCaption}.");
            }
            else
            {
                // А был ли пользователь в этой очереди?
                int? user = null;
                if (productProcessing.UserIds != null)
                {
                    user = productProcessing.UserIds.Single(u => u == order.UserId);
                    productProcessing.UserIds.Remove(order.UserId);
                }
                
                if (productProcessing.UserIds == null)
                {
                    productProcessing.UserIds = new List<int>();
                }

                // Добавляем старого или нового ожидающего пользователя в конец очереди.
                productProcessing.UserIds.Add(order.UserId);

                // Передаём обновлённый процессинг в сервисы мониторинга продуктов.
                _monitorPrime.SetProductProcessing(productProcessing);
                _monitorSecundo.SetProductProcessing(productProcessing);

                var msg = new InformationMessage
                {
                    aggregateId = Guid.NewGuid(),
                    ProductName = order.ProductCaption,
                    eventDate = DateTime.Now,
                    UserId = order.UserId,
                };

                // Отправляем уведомление.
                _informSrv.publicateInformation(msg);
            }
        }
    }
}
