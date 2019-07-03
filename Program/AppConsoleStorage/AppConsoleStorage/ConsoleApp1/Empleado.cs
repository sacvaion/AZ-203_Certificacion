using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Empleado : TableEntity
    {
        public Empleado() { }
        public Empleado(string group)
        {
            this.PartitionKey = group;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string NoDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string NumeroCelular { get; set; }
        public string UrlFoto { get; set; }
        public Dependencia Dependencia { get; set; }
        public TipoContrato TipoContrato { get; set; }

    }

    public class Dependencia
    {
        public Dependencia() { }
        public int IdDependencia { get; set; }
        public string Descripcion { get; set; }

    }

    public class TipoContrato
    {
        public TipoContrato() { }
        public int IdTipoContrato { get; set; }
        public string Descripcion { get; set; }

    }
}
