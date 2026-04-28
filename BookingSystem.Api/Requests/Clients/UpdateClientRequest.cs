namespace BookingSystem.Api.Requests.Clients;

public class UpdateClientRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
