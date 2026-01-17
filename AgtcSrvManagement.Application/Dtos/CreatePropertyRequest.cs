using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgtcSrvManagement.Application.Dtos;

public class CreatePropertyRequest
{
    [Required(ErrorMessage = "O nome da propriedade é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome da propriedade deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "A localização da propriedade é obrigatória.")]
    [StringLength(150, ErrorMessage = "A localização da propriedade deve ter no máximo 150 caracteres.")]
    public string Location { get; set; }

}
