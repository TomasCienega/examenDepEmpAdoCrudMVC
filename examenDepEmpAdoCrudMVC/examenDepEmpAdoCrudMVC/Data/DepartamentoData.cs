using examenDepEmpAdoCrudMVC.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace examenDepEmpAdoCrudMVC.Data
{
    public class DepartamentoData
    {
        private readonly string _cadenaConn = "";
        public DepartamentoData(string cadenaConn)
        {
            _cadenaConn = cadenaConn;
        }


        #region
        public async Task<List<Departamento>> ListarDepartamentos()
        {
            var _listarDeps = new List<Departamento>();

            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_ListarDepartamentos", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        _listarDeps.Add(new Departamento
                        {

                            IdDepartamento = Convert.ToInt32(dr["idDepartamento"]),
                            NombreDepartamento = dr["nombreDepartamento"].ToString() ?? ""

                        });
                    }
                }
            }
            return _listarDeps;
        }
        #endregion
    }
}
