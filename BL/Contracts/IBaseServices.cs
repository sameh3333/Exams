using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IBaseServices<T, DTO>
    {
        List<DTO> GetAll();
        DTO GetById(Guid id);  // أو Domines.Guid إذا كنت تستخدمه
        bool Add(DTO entity, Guid userId);
        bool Update(DTO entity, Guid userId);
        bool ChangeStatus(Guid id, Guid userId, int status = 1);  // أو Domines.Guid
    }
}
