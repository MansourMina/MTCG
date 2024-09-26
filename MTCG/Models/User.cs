using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{

    public class User
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public List<Card>? cards { get; set; }
        public List<Stack>? stack { get; set; }
    }
}
