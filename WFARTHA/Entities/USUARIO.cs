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
    
    public partial class USUARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIO()
        {
            this.DELEGARs = new HashSet<DELEGAR>();
            this.DELEGARs1 = new HashSet<DELEGAR>();
            this.DET_AGENTEC = new HashSet<DET_AGENTEC>();
            this.DET_AGENTEC1 = new HashSet<DET_AGENTEC>();
            this.DET_SOCIEDAD = new HashSet<DET_SOCIEDAD>();
            this.DET_TIPOPRESUPUESTO = new HashSet<DET_TIPOPRESUPUESTO>();
            this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            this.FLUJOes = new HashSet<FLUJO>();
            this.FLUJOes1 = new HashSet<FLUJO>();
            this.FOROes = new HashSet<FORO>();
            this.FORODs = new HashSet<FOROD>();
            this.MIEMBROS = new HashSet<MIEMBRO>();
            this.WORKFHs = new HashSet<WORKFH>();
        }
    
        public string ID { get; set; }
        public string PASS { get; set; }
        public string NOMBRE { get; set; }
        public string APELLIDO_P { get; set; }
        public string APELLIDO_M { get; set; }
        public string EMAIL { get; set; }
        public string SPRAS_ID { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public Nullable<int> PUESTO_ID { get; set; }
        public string MANAGER { get; set; }
        public string BACKUP_ID { get; set; }
        public string BUNIT { get; set; }
        public string FIRMA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DELEGAR> DELEGARs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DELEGAR> DELEGARs1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_AGENTEC> DET_AGENTEC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_AGENTEC> DET_AGENTEC1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_SOCIEDAD> DET_SOCIEDAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_TIPOPRESUPUESTO> DET_TIPOPRESUPUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FLUJO> FLUJOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FLUJO> FLUJOes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORO> FOROes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FOROD> FORODs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MIEMBRO> MIEMBROS { get; set; }
        public virtual PUESTO PUESTO { get; set; }
        public virtual SPRA SPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WORKFH> WORKFHs { get; set; }
    }
}
