using BL.Contracts;
using Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Exams.Contracts;
using AutoMapper;
using BL.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace BL.Services
{
    public class BaseServices<T, DTO> : IBaseServices<T, DTO> where T : BaseTable
    {
          readonly IGenericRepository<T> _redo;
       
        readonly IMapper _mapper;

        public BaseServices(IGenericRepository<T> redo, IMapper mapper)
        {
            _mapper = mapper;
            _redo = redo;
        }
        public List<DTO> GetAll()
        {
            var LIST = _redo.GetAll();
            return _mapper.Map<List<T>, List<DTO>>(LIST);
        }

        public DTO GetById(Guid id)
        {
            var opj = _redo.GetById(id);
            return _mapper.Map<T, DTO>(opj);
        }
        public bool Add(DTO entity, Guid userId)
        {
            var dbopject = _mapper.Map<DTO, T>(entity);
            // dbopject.CreatedBy = userId;
            if (dbopject.Id == Guid.Empty)
                dbopject.Id = Guid.NewGuid();
            dbopject.CreatedBy = new Guid();
            dbopject.UpdatedDate = DateTime.Now;
            dbopject.UpdatedBy = userId;
            dbopject.CreatedDate = DateTime.Now;

            dbopject.CurrentState = 1;
            return _redo.Add(dbopject);

        }
        

        public bool Update(DTO entity, Guid userId)
        {
            var dbopject = _mapper.Map<DTO, T>(entity);
            dbopject.UpdatedBy = (userId);
            dbopject.CurrentState = 1;
            //   dbopject.CreatedDate = DateTime.Now;
            return _redo.Update(dbopject);


        }
        public bool ChangeStatus(Guid id, Guid userId, int status = 1)
        {
            return _redo.ChangeStatus(id, 0);
        }


    }

}
