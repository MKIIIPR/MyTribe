using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribe.Bib.Models.TribeRelated;


namespace Tribe.Bib.CommunicationModels
{
    public  class ComModels
    {
        public class LoginRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class BillingAddress
        {
            [Key]
            public string Guid { get; set; } = System.Guid.NewGuid().ToString();
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        public class PaymentInfo
        {
            [Key]
            public string Guid { get; set; } = System.Guid.NewGuid().ToString();
            public string PaymentMethod { get; set; } = "CreditCard";
            public string CardNumber { get; set; } = string.Empty;
            public string CardHolderName { get; set; } = string.Empty;
            public string ExpiryDate { get; set; } = string.Empty;
            public string CvvCode { get; set; } = string.Empty;
            public bool AcceptTerms { get; set; } = false;
            public bool AcceptRecurring { get; set; } = false;
        }

      
        public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? ProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
 
  

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class TestApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
    }

}
}
