using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IBaseServices<T, DTO>
    {
      // أو Domines.Guid إذا كنت تستخدمه
        Task<List<DTO>> GetAll();
        Task<DTO> GetById(Guid id);


       Task< bool> Add(DTO entity);
        Task<bool> Update(DTO entity);
        Task<bool> ChangeStatus(Guid id, Guid userId, int status = 1);  // أو Domines.Guid
    }
}
