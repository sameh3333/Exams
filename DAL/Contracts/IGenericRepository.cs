using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin;
namespace Exams.Contracts
{

    public interface IGenericRepository<T> where T : BaseTable
    {
        Task<List<T>> GetAll();
        Task<T> GetById(Guid id);  // أو Domines.Guid إذا كنت تستخدمه`
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> ChangeStatus(Guid id, int status = 1);  // أو Domines.Guid


        
    }
}
