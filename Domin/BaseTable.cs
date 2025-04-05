using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin
{
    public class BaseTable
    {


        public Guid Id { get; set; }


        public DateTime? CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public int CurrentState { get; set; }

    }
}
