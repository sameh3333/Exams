
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BL.Exceptions;
using Exams.Contracts;
using DAL.Context;
using Domin;

namespace DAL.Repositorys
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseTable
    {
        private readonly ExamsContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(ExamsContext context, ILogger<GenericRepository<T>> log)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = log;
        }

        public  async Task<List<T>> GetAll()
        {
            try
            {
                //return  _dbSet.Where(a => a.CurrentState > 0).AsNoTracking().ToList();
                return await _dbSet.Where(a => a.CurrentState > 0).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }
        // الحصول على كائن بواسطة الـ Idtype 'BL.Exceptions.DataAccessException' was 
        public async Task<T> GetById(Guid id)
        {
            try
            {
                return await _dbSet.Where(a => a.Id == id)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }
        // إضافة كائن جديد
        public async Task<bool> Add(T entity)
        {
            try
            {
                entity.CreatedDate = DateTime.Now;
                 _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }

        // تحديث كائن موجود
        public async Task<bool> Update(T entity)
        {
            try
            {


                var dbData =await GetById(entity.Id);
                //_context.Entry(dbData).State = EntityState.Detached; // فصل الكيان القديم

                entity.CreatedDate = dbData.CreatedDate;
                entity.CreatedBy = dbData.CreatedBy;
                entity.UpdatedDate = DateTime.Now;
                entity.CurrentState = 1;
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }
        // حذف كائن (يتم حذف الكائن من خلال تغيير حالته بدلًا من حذفه فعليًا)
        public async Task<bool> Delete(T entity)
        {
            try
            {
                // الحذف المنطقي بتغيير حالة الكائن إلى 0 (غير نشط)
                _context.Entry(entity).State = EntityState.Modified;
             await   _context.SaveChangesAsync();
                return true;
            }

            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }
        // تغيير حالة الكائن (على سبيل المثال تفعيل أو إلغاء تفعيل)
        public async Task<bool> ChangeStatus(Guid id, Guid userId, int status = 1)
        {
            try
            {
                var entity = await GetById(id);
                if (entity != null)
                {
                    entity.CurrentState = status;
                    entity.UpdatedBy = userId;
                    entity.UpdatedDate = DateTime.Now;
                    _context.Entry(entity).State = EntityState.Modified;
                  await  _context.SaveChangesAsync();
                    return true;
                }
                return false;  // في حال لم يتم العثور على الكائن
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, "", _logger);
            }
        }

        
    }

}
