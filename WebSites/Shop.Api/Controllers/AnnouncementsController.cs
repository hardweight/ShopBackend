using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Announcements;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Announcements;
using Shop.Commands.Announcements;
using Shop.ReadModel.Announcements;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 公告
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
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
            var announcements = _announcementQueryService.ListPage().OrderByDescending(x=>x.CreatedOn);
            return new ListResponse
            {
                Total = announcements.Count(),
                Announcements = announcements.Select(x=>new Announcement {
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
            var announcement = _announcementQueryService.ListPage().OrderByDescending(x=>x.CreatedOn).Take(1).SingleOrDefault();
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

        #region 后台管理

        /// <summary>
        /// 添加公告
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("AnnouncementAdmin/Add")]
        public async Task<BaseApiResponse> Add(AddAnnouncementRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CreateAnnouncementCommand(
                GuidUtil.NewSequentialId(),
                request.Title,
                request.Body
                );
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("AnnouncementAdmin/Edit")]
        public async Task<BaseApiResponse> Edit(EditAnnouncementRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateAnnouncementCommand(
                request.Title,
                request.Body
                )
            {
                AggregateRootId=request.Id
            };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("AnnouncementAdmin/ListPage")]
        public BaseApiResponse ListPage()
        {
            var announcements = _announcementQueryService.ListPage();
            return new ListResponse
            {
                Total=announcements.Count(),
                Announcements = announcements.Select(x => new Announcement
                {
                    Id = x.Id,
                    Title = x.Title,
                    Body = x.Body
                }).ToList()
            };
        }
        #endregion



        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}