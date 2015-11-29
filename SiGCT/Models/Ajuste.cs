﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiGCT.Models
{
    public class Ajuste
    {
        public virtual String Id { get; set; }

        public virtual Conta Fatura { get; set; }

        public virtual Recurso Recurso { get; set; }

        public virtual TipoAssociadoEnum Tipo { get; set; }

        public virtual Categoria Categoria { get; set; }

        public virtual long BaseCalculo { get; set; }

        public virtual long Percentual { get; set; }

        public virtual long Sinal { get; set; }

        public virtual long Valor { get; set; }

        public virtual DateTime Inicio { get; set; }

        public virtual DateTime Fim { get; set; }

        [MaxLength(15)]
        public virtual String Filler { get; set; }

        [MaxLength(25)]
        public virtual String Obs { get; set; }
    }
}