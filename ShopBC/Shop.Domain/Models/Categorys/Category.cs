using ENode.Domain;
using Shop.Domain.Events.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xia.Common.Extensions;

namespace Shop.Domain.Models.Categorys
{
    /// <summary>
    /// 分类聚合跟
    /// </summary>
    public class Category: AggregateRoot<Guid>
    {
        private string _name;
        private string _url;
        private string _thumb;
        private Guid _parentId;

        public Category(Guid id,Category parent,string name,string url,string thumb):base(id)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            url.CheckNotNullOrEmpty(nameof(url));
            ApplyEvent(new CategoryCreatedEvent(parent == null ? Guid.Empty : parent.Id,name, url,thumb));
        }

        public void UpdateCategory(string name,string url,string thumb)
        {
            ApplyEvent(new CategoryUpdatedEvent(name,url,thumb));
        }


        #region Handle

        
        private void Handle(CategoryCreatedEvent evnt)
        {
            _parentId = evnt.ParentId;
            _name = evnt.Name;
            _url = evnt.Url;
            _thumb = evnt.Thumb;
        }
        private void Handle(CategoryUpdatedEvent evnt)
        {
            _name = evnt.Name;
            _url = evnt.Url;
            _thumb = evnt.Thumb;
        }
        #endregion
    }
}
