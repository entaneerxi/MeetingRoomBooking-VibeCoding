using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomBooking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Booked By")]
        public string BookedBy { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Meeting Title")]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Booking Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookingDate => StartTime.Date;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Number of Attendees")]
        [Range(1, 500)]
        public int NumberOfAttendees { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation property
        public Room Room { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}