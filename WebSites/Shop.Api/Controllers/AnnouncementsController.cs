using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Announcements;
using Shop.ReadModel.Announcements;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 公告
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://app.wftx666.com,http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class AnnouncementController : BaseApiController
    {
        private ICommandService _commandService;//C端
        private AnnouncementQueryService _announcementQueryService;

        public AnnouncementController(ICommandService commandService, 
            AnnouncementQueryService announcementQueryService)
        {
            _commandService = commandService;
            _announcementQueryService = announcementQueryService;
        }

        #region 公告列表

        /// <summary>
        /// 产品列表页面
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Announcement/List")]
        public BaseApiResponse List()
        {
            var announcements = _announcementQueryService.ListPage();
            return new ListResponse
            {
                Announcements= announcements.Select(x=>new Announcement {
                    Id=x.Id,
                    Title=x.Title,
                    Body=x.Body
                }).ToList()
            };
        }

        /// <summary>
        /// 获取最新公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Announcement/LatestAnnouncement")]
        public BaseApiResponse LatestAnnouncement()
        {
            var announcement = _announcementQueryService.ListPage().Take(1).SingleOrDefault();
            if(announcement==null)
            {
                return new BaseApiResponse { Code = 400, Message = "没有公告" };
            }
            return new LatestAnnouncementResponse
            {
                Announcement = new Announcement
                {
                    Id = announcement.Id,
                    Title = announcement.Title,
                    Body = announcement.Body
                }
            };
        }
        
        #endregion



        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}