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
        List<T> GetAll();
        T GetById(Guid id);  // أو Domines.Guid إذا كنت تستخدمه`
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool ChangeStatus(Guid id, int status = 1);  // أو Domines.Guid


        
    }
}
