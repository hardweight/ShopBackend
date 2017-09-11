using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.PubCategorys;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.PubCategorys;
using Shop.Commands.PubCategorys;
using Shop.ReadModel.PubCategorys;
using Shop.ReadModel.PubCategorys.Dtos;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class PubCategoryController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private PubCategoryQueryService _pubCategoryQueryService;//Q 端

        public PubCategoryController(ICommandService commandService, 
            PubCategoryQueryService pubCategoryQueryService)
        {
            _commandService = commandService;
            _pubCategoryQueryService = pubCategoryQueryService;
        }

        /// <summary>
        /// 获取类别树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("PubCategory/CategoryTree")]
        public BaseApiResponse CategoryTree()
        {
            //递归获取分类包含子类
            Func<PubCategory, object> getNodeData = null;

            getNodeData = cat => {
                dynamic node = new ExpandoObject();
                node.Id = cat.Id;
                node.Name = cat.Name;
                node.Thumb = cat.Thumb;
                node.Children = new List<dynamic>();

                var childrens = _pubCategoryQueryService.GetChildren(cat.Id).OrderByDescending(x=>x.Sort);
                foreach (var child in childrens)
                {
                    node.Children.Add(getNodeData(child));
                }
                return node;
            };

            List<PubCategory> rootsCategory = _pubCategoryQueryService.RootCategorys().OrderByDescending(x=>x.Sort).ToList();
            List<object> nodes = rootsCategory.Select(getNodeData).ToList();

            return new PubCategoryTreeResponse
            {
                Categorys = nodes
            };
        }


        #region 管理


        /// <summary>
        /// 添加类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("PubCategoryAdmin/Add")]
        public async Task<BaseApiResponse> Add(AddPubCategoryRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CreatePubCategoryCommand(
                GuidUtil.NewSequentialId(),
                request.ParentId,
                request.Name,
                request.Thumb,
                request.Sort);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();

        }

        /// <summary>
        /// 编辑类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PubCategoryAdmin/Update")]
        public async Task<BaseApiResponse> Update(UpdatePubCategoryRequest request)
        {
            request.CheckNotNull(nameof(request));
            var command = new UpdatePubCategoryCommand(
                request.Name,
                request.Thumb,
                request.Sort)
            {
                AggregateRootId = request.Id
            };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        

        /// <summary>
        /// 获取类别树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("PubCategoryAdmin/CategoryTree")]
        public BaseApiResponse AdminCategoryTree()
        {
            //递归获取分类包含子类
            Func<PubCategory, object> getNodeData = null;

            getNodeData = cat => {
                dynamic node = new ExpandoObject();
                node.Id = cat.Id;
                node.Name = cat.Name;
                node.Thumb = cat.Thumb;
                node.Sort = cat.Sort;
                node.Children = new List<dynamic>();

                var childrens = _pubCategoryQueryService.GetChildren(cat.Id).OrderByDescending(x=>x.Sort);
                foreach (var child in childrens)
                {
                    node.Children.Add(getNodeData(child));
                }
                return node;
            };

            List<PubCategory> rootsCategory = _pubCategoryQueryService.RootCategorys().OrderByDescending(x=>x.Sort).ToList();
            List<object> nodes = rootsCategory.Select(getNodeData).ToList();

            return new PubCategoryTreeResponse
            {
                Categorys = nodes
            };
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("PubCategoryAdmin/ListPage")]
        public ListPageResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));
            var categorys = _pubCategoryQueryService.RootCategorys();
            return new ListPageResponse
            {
                Total=categorys.Count(),
                Categorys = categorys.Select(x => new Category
                {
                    Id = x.Id,
                    Name = x.Name,
                    Thumb = x.Thumb,
                    Sort = x.Sort
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