using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Loan> GetLoans()
        {
            return FindAll().ToList();
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id).FirstOrDefault();
        }

        public double GetMaxAmmountById(long id)
        {
            return FindById(id).MaxAmount;
        }

        public string GetPaymentsById(long id)
        {
            return FindById(id).Payments;
        }

        public bool IsPaymentValid(long loanId, int payment)
        {
            string paymentsString = GetPaymentsById(loanId); // Obtener Payments del préstamo

            if (!string.IsNullOrEmpty(paymentsString))
            {
                // Separar el string en un array de números
                string[] paymentsArray = paymentsString.Split(',');

                // Verificar si el intervalToCheck está en el array de pagos
                if (paymentsArray.Contains(payment.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
