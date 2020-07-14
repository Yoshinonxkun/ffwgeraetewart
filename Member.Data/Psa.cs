using System;
using System.ComponentModel.DataAnnotations;

namespace Member.Data
{
    public class Psa
    {
        [Key] public int PsaId { get; set; }

        public int EinsatzJacke { get; set; }
        public int EinsatzHose { get; set; }
        public int ArbeitsJacke { get; set; }
        public int ArbeitsHose { get; set; }
        public int Helm { get; set; }
        public DateTime HelmDate { get; set; }
        public int Handschuhe { get; set; }
        public int Schuhe { get; set; }
        public int Kopfschutzhaube { get; set; }

        [Required] public bool IsDeleted { get; set; }
    }
}