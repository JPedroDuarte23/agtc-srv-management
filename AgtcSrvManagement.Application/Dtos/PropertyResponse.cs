using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgtcSrvManagement.Application.Dtos;

public class PropertyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public double TotalArea { get; set; }
    public Guid OwnerId { get; set; }
    public List<FieldResponse> Fields { get; set; } = new();
}
