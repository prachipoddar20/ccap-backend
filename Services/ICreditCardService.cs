using CCAP.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCAP.Api.Services
{
    public interface ICreditCardService
    {
        Task<IList<CreditCardDisplayViewModel>> Get();
        Task<CreditCardDisplayViewModel> Get(int id);
    }
}
