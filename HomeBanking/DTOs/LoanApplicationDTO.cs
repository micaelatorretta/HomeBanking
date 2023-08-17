using System.ComponentModel.DataAnnotations;

namespace HomeBanking.DTOs
{
    public class LoanApplicationDTO
    {
        [Required(ErrorMessage = "El Id de Loan es obligatorio.")]
        public long LoanId { get; set; }

        [Required(ErrorMessage = "El campo Amount es obligatorio.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "El campo Payments es obligatorio.")]
        public int Payments { get; set; }

        [Required(ErrorMessage = "El campo ToAccountNumber es obligatorio.")]
        public string ToAccountNumber { get; set; }

    }
}
