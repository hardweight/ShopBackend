using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Categorys;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Categorys;
using Shop.Commands.Categorys;
using Shop.ReadModel.Categorys;
using Dtos=Shop.ReadModel.Categorys.Dtos;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;
using Xia.Common;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class CategoryController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private CategoryQueryService _categoryQueryService;//Q 端

        public CategoryController(ICommandService commandService,
            CategoryQueryService categoryQueryService)
        {
            _commandService = commandService;
            _categoryQueryService = categoryQueryService;
        }


        /// <summary>
        /// 获取类别树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Category/CategoryTree")]
        public BaseApiResponse CategoryTree()
        {
            //递归获取分类包含子类
            Func<Dtos.Category, object> getNodeData = null;

            getNodeData = cat => {
                dynamic node = new ExpandoObject();
                node.Id = cat.Id;
                node.Name = cat.Name;
                node.Thumb = cat.Thumb;
                node.Url = cat.Url;
                node.Type = cat.Type.ToString();
                node.IsShow = cat.IsShow;
                node.Sort = cat.Sort;
                node.Children = new List<dynamic>();

                var childrens = _categoryQueryService.GetChildren(cat.Id).Where(x=>x.IsShow).OrderByDescending(x=>x.Sort);
                foreach (var child in childrens)
                {
                    node.Children.Add(getNodeData(child));
                }
                return node;
            };

            List<Dtos.Category> rootsCategory = _categoryQueryService.RootCategorys().Where(x=>x.IsShow).OrderByDescending(x=>x.Sort).ToList();
            List<object> nodes = rootsCategory.Select(getNodeData).ToList();

            return new CategoryTreeResponse
            {
                Categorys = nodes
            };
        }


        #region 管理
        /// <summary>
        /// 获取类别树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CategoryAdmin/CategoryTree")]
        public BaseApiResponse AdminCategoryTree()
        {
            //递归获取分类包含子类
            Func<Dtos.Category, object> getNodeData = null;

            getNodeData = cat => {
                dynamic node = new ExpandoObject();
                node.Id = cat.Id;
                node.Name = cat.Name;
                node.Thumb = cat.Thumb;
                node.Url = cat.Url;
                node.Type = cat.Type.ToString();
                node.IsShow = cat.IsShow;
                node.Sort = cat.Sort;
                node.Children = new List<dynamic>();

                var childrens = _categoryQueryService.GetChildren(cat.Id).OrderByDescending(x=>x.Sort);
                foreach (var child in childrens)
                {
                    node.Children.Add(getNodeData(child));
                }
                return node;
            };

            List<Dtos.Category> rootsCategory = _categoryQueryService.RootCategorys().OrderByDescending(x=>x.Sort).ToList();
            List<object> nodes = rootsCategory.Select(getNodeData).ToList();

            return new CategoryTreeResponse
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
        [Route("CategoryAdmin/ListPage")]
        public ListPageResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));
            var categorys = _categoryQueryService.RootCategorys();
            return new ListPageResponse
            {
                Total = categorys.Count(),
                Categorys = categorys.Select(x => new Category
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url=x.Url,
                    Thumb = x.Thumb,
                    Type=x.Type.ToString(),
                    IsShow=x.IsShow,
                    Sort = x.Sort
                }).ToList()
            };
        }

        /// <summary>
        /// 添加类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CategoryAdmin/Add")]
        public async Task<BaseApiResponse> Add(AddCategoryRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CreateCategoryCommand(
                GuidUtil.NewSequentialId(),
                request.ParentId,
                request.Name,
                request.Url,
                request.Thumb,
                request.Type,
                request.IsShow,
                request.Sort);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();

        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CategoryAdmin/Delete")]
        public async Task<BaseApiResponse> Delete(DeleteRequest request)
        {
            request.CheckNotNull(nameof(request));
            //分类判断
            var category = _categoryQueryService.Find(request.Id);
            if(category==null)
            {
                return new BaseApiResponse { Code = 400, Message = "没找到该分类" };
            }
            //判断是否有子分类
            var children = _categoryQueryService.GetChildren(request.Id);
            if (children.Any())
            {
                return new BaseApiResponse { Code = 400, Message = "包含子分类，无法删除" };
            }
            //删除
            var command = new DeleteCategoryCommand(request.Id);
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
        [Route("CategoryAdmin/Update")]
        public async Task<BaseApiResponse> Update(UpdateCategoryRequest request)
        {
            request.CheckNotNull(nameof(request));
            var command = new UpdateCategoryCommand(
                request.Name,
                request.Url,
                request.Thumb,
                request.Type,
                request.IsShow,
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
        #endregion


        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}