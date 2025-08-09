using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 500)]
        public int Capacity { get; set; }

        [Display(Name = "Has Projector")]
        public bool HasProjector { get; set; }

        [Display(Name = "Has Video Conference")]
        public bool HasVideoConference { get; set; }

        [Display(Name = "Floor Number")]
        public int FloorNumber { get; set; }

        [Display(Name = "Room Number")]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        // Navigation property
        public ICollection<Booking> Bookings { get; set; }
    }
}