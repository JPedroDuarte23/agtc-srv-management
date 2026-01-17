using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgtcSrvManagement.Application.Dtos;

public class CreateFieldRequest
{
    [Required(ErrorMessage = "O nome do talhão é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do talhão deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O tipo de cultivo do talhão é obrigatório.")]
    [StringLength(100, ErrorMessage = "O tipo de cultivo do talhão deve ter no máximo 100 caracteres.")]
    public string CropType { get; set; }

    [Required(ErrorMessage = "A área do talhão é obrigatória.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "A área do talhão deve ser maior que zero.")]
    public double Area { get; set; }
}
