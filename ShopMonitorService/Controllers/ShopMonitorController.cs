using Microsoft.AspNetCore.Mvc;
using ShopMonitorService.OrchServiceSrv;
using ShopMonitorService.ProductProcessingData;

namespace ShopMonitorService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopMonitorController : Controller
    {
        private readonly ILogger<ShopMonitorController> _log;
        private List<ProductProcessing> _processing;
        private readonly OrchService _orchService;

        /// <summary>
        /// Контроллер сервиса мониторинга сайтов.
        /// </summary>
        /// <param name="log">Логирование сообщений.</param>
        public ShopMonitorController(ILogger<ShopMonitorController> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _processing = new List<ProductProcessing>();
            _orchService = OrchService.GetSingleSrv("http://localhost:5270/");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="processingRequest">Добавление нового продуктового процессинга для конкретного товара (продукта). Если процессинг существует - обновляется очередь.</param>
        [HttpPost(Name = "runprocessing")]
        public void SetProductProcessing(ProductProcessing processingRequest)
        {
            var productProcessing = _processing.First(p => p.ProductCaption == processingRequest.ProductCaption);

            if (productProcessing == null)
            {
                _processing.Add(new ProductProcessing
                {
                    ProductCaption = processingRequest.ProductCaption,
                    UserIds = processingRequest.UserIds,
                });
            }
            else
            {
                productProcessing.UserIds = processingRequest.UserIds;
            }
        }

        /// <summary>
        /// Поступление экземпляра продукта для распределения.
        /// В данной реализации при отсутствии спроса единица товара не кешируется.
        /// </summary>
        /// <param name="productCaption"></param>
        [HttpPost(Name = "registerproduct")]
        public void SetToProcessing(string productCaption)
        {
            var productProcessing = _processing.First(p => p.ProductCaption == productCaption);

            if (productProcessing == null)
            {
                _log.LogInformation($"{DateTime.Now}: нет запросов потребителей на продукт {productCaption}.");
            }
            else
            {
                // Резервируем продукт за пользователем, который первым был в именной продуктовой очереди.
                int userId = productProcessing.UserIds.First();
                _orchService.ReserveProductToUser(userId);
            }
        }
    }
}
