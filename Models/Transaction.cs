using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Please select a category.")]
        public int CategoryId { get; set; }
        public Category ?Category { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid amount.")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string?  Note { get; set; }

        public DateTime Date { get; set; }  = DateTime.Now;

        [NotMapped]
        public string? CategoryWithIcon {
            get{ 
                return Category ==null? "" :Category.Icon + " " + Category.Title;
            }
                }

        [NotMapped]
        public string FormatedAmount
        {
            get
            {
                return (Category == null || Category.Type == "Expense") ? "- " + Amount.ToString("C0") : "+ " + Amount.ToString("C0");
            }
        }

    }
}
