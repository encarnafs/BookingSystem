using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Rooms.Dtos;
public class RoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}
