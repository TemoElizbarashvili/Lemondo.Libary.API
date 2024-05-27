using AutoMapper;
using Lemondo.Libary.API.UnitOfWork.Contract;
using Microsoft.AspNetCore.Mvc;

namespace Lemondo.Libary.API.Modules.Shared.Controller;
public class BaseController(IUnitOfWork uow, IMapper mapper) : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork = uow;
    protected readonly IMapper _mapper = mapper;
}
