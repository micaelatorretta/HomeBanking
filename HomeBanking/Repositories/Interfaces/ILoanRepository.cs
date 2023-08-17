using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetLoans();
        Loan FindById(long id);
        double GetMaxAmmountById(long id);
        string GetPaymentsById(long id);
        bool IsPaymentValid(long loanId, int payment);
    }
}
