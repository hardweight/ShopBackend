using ENode.Domain;
using Shop.Domain.Events.PubCategorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xia.Common.Extensions;

namespace Shop.Domain.Models.PubCategorys
{
    /// <summary>
    /// 商品发布分类聚合跟
    /// </summary>
    public class PubCategory: AggregateRoot<Guid>
    {
        private string _name;
        private string _thumb;
        private int _sort;
        private Guid _parentId;

        
        public PubCategory(Guid id,PubCategory parent,string name,string thumb,int sort):base(id)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            ApplyEvent(new PubCategoryCreatedEvent(parent == null ? Guid.Empty : parent.Id,name,thumb,sort));
        }

        public void UpdateCategory(string name,string thumb,int sort)
        {
            ApplyEvent(new PubCategoryUpdatedEvent(name,thumb,sort));
        }


        

        #region Handle

        
        private void Handle(PubCategoryCreatedEvent evnt)
        {
            _parentId = evnt.ParentId;
            _name = evnt.Name;
            _thumb = evnt.Thumb;
            _sort = evnt.Sort;
        }
        private void Handle(PubCategoryUpdatedEvent evnt)
        {
            _name = evnt.Name;
            _thumb = evnt.Thumb;
            _sort = evnt.Sort;
        }
        #endregion
    }
}
