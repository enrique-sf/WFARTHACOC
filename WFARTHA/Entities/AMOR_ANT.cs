//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFARTHA.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AMOR_ANT
    {
        public int POS { get; set; }
        public decimal NUM_DOC { get; set; }
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public string BELNR { get; set; }
        public Nullable<decimal> GJAHR { get; set; }
        public Nullable<decimal> BUZEI { get; set; }
        public Nullable<decimal> ANTAMOR { get; set; }
        public Nullable<decimal> TANT { get; set; }
        public string WAERS { get; set; }
        public Nullable<decimal> ANTTRANS { get; set; }
        public Nullable<decimal> ANTXAMORT { get; set; }
    
        public virtual DOCUMENTO DOCUMENTO { get; set; }
        public virtual EKPO EKPO { get; set; }
    }
}