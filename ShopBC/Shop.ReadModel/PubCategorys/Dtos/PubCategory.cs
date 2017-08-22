using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.ReadModel.PubCategorys.Dtos
{
    public class PubCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Thumb { get; set; }
        public int Sort { get; set; }

    }
}
