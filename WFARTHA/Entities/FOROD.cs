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
    
    public partial class FOROD
    {
        public byte ID_RESPUESTA { get; set; }
        public byte ID_FORO { get; set; }
        public string ID_USUARIO { get; set; }
        public System.DateTime CREADO { get; set; }
        public string COMENTARIO { get; set; }
        public Nullable<bool> MAIL { get; set; }
    
        public virtual FORO FORO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}