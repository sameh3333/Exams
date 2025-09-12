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
        private readonly IUserServices _userServices;

        public BaseServices(IGenericRepository<T> redo, IMapper mapper, IUserServices userServices)
        {
            _mapper = mapper;
            _redo = redo;
            _userServices = userServices;
        }

        

        public async Task<List<DTO>> GetAll()
        {
            var LIST =await _redo.GetAll();
            return _mapper.Map<List<T>, List<DTO>>(LIST);
        }

        public async Task<DTO> GetById(Guid id)
        {
            var opj = await _redo.GetById(id);
            return _mapper.Map<T, DTO>(opj);
        }
        public async Task<bool> Add(DTO entity)
        {
            var dbopject = _mapper.Map<DTO, T>(entity);
            // dbopject.CreatedBy = userId;
            if (dbopject.Id == Guid.Empty)
                dbopject.Id = Guid.NewGuid();
            dbopject.CreatedBy = _userServices.GetLoggedInServices();
            dbopject.UpdatedDate = DateTime.Now;
            dbopject.CreatedDate = DateTime.Now;

            dbopject.CurrentState = 1;
            return await _redo.Add(dbopject);

        }
        

        public async Task<bool> Update(DTO entity)
        {
            var dbopject = _mapper.Map<DTO, T>(entity);
            dbopject.UpdatedBy = _userServices.GetLoggedInServices();
            dbopject.CurrentState = 1;
            //   dbopject.CreatedDate = DateTime.Now;
            return await _redo.Update(dbopject);


        }
        public async Task<bool> ChangeStatus(Guid id, Guid userId, int status = 1)
        {
            return await _redo.ChangeStatus(id, userId, status);
        }


    }

}
