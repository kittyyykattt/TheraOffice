using System;

namespace Maui.TheraOffice.Models
{
    public class Treatment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AppointmentId { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }
}
