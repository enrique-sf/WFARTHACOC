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
    
    public partial class EKPO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EKPO()
        {
            this.AMOR_ANT = new HashSet<AMOR_ANT>();
        }
    
        public string EBELN { get; set; }
        public string EBELP { get; set; }
        public Nullable<System.DateTime> BEDAT { get; set; }
        public string MATNR { get; set; }
        public string TXZ01 { get; set; }
        public string MATKL { get; set; }
        public string WERKS { get; set; }
        public string LGORT { get; set; }
        public Nullable<decimal> MENGE { get; set; }
        public string MEINS { get; set; }
        public Nullable<decimal> NETPR { get; set; }
        public string WAERS { get; set; }
        public Nullable<decimal> PEINH { get; set; }
        public Nullable<decimal> MENGE_DEL { get; set; }
        public Nullable<decimal> NETPR_DEL { get; set; }
        public Nullable<decimal> MENGE_BIL { get; set; }
        public Nullable<decimal> NETPR_BIL { get; set; }
        public string MWSKZ { get; set; }
        public string SAKTO { get; set; }
        public string KNTTP { get; set; }
        public string PS_PSP_PNR { get; set; }
        public string KOSTL { get; set; }
        public string ESTATUS { get; set; }
        public string EREKZ { get; set; }
        public Nullable<decimal> NETWR { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMOR_ANT> AMOR_ANT { get; set; }
    }
}
