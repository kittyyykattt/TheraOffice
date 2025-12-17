using System;

namespace Maui.TheraOffice.Models
{
    public class Diagnosis
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AppointmentId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
