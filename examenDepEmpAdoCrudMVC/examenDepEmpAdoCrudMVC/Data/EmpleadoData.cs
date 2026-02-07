using examenDepEmpAdoCrudMVC.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace examenDepEmpAdoCrudMVC.Data
{
    public class EmpleadoData
    {
        #region
        //version 1 en esta version no ocupas hacer nada en el program mas que agregar el alcance a tus data
        //private readonly string _cadenasSQL = "";
        //public EmpleadoData(IConfiguration configuration)
        //{
        //    _cadenasSQL = configuration.GetConnectionString("cadenaSQL") ?? string.Empty;
        //}
        #endregion

        private readonly string _cadenaConn = "";
        public EmpleadoData(string cadenaConn)
        {
            _cadenaConn = cadenaConn;
        }

        #region
        public async Task<List<Empleado>> ListarEmpleados()
        {
            var _listarEmpls = new List<Empleado>();

            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_ListarEmpleados", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        _listarEmpls.Add(new Empleado
                        {
                            IdEmpleado = Convert.ToInt32(dr["idEmpleado"]),
                            NombreEmpleado = dr["nombreEmpleado"].ToString() ?? "",
                            ReferenciaDepartamento = new Departamento
                            {
                                IdDepartamento = Convert.ToInt32(dr["idDepartamento"]),
                                NombreDepartamento = dr["nombreDepartamento"].ToString() ?? ""
                            },
                            Activo = Convert.ToBoolean(dr["activo"])
                        });
                    }
                }
            }
            return _listarEmpls;
        }
        #endregion

        #region
        public async Task<List<Empleado>> ListarEmpPorIdDep(int IdDep)
        {
            var _listaEmpPorIdDep = new List<Empleado>();
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_ListarEmpleadoPorIdDepartamento", conn);
                cmd.Parameters.AddWithValue("@idDepartamento", IdDep);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        _listaEmpPorIdDep.Add(new Empleado
                        {
                            IdEmpleado = Convert.ToInt32(dr["idEmpleado"]),
                            NombreEmpleado = dr["nombreEmpleado"].ToString() ?? "",
                            ReferenciaDepartamento = new Departamento
                            {
                                IdDepartamento = Convert.ToInt32(dr["idDepartamento"]),
                                NombreDepartamento = dr["nombreDepartamento"].ToString() ?? ""
                            },
                            Activo = Convert.ToBoolean(dr["activo"])
                        });
                    }
                }
            }
            return _listaEmpPorIdDep;
        }
        #endregion

        #region
        public async Task<Empleado?> ListarEmpPorIdEmpl(int IdEmpl)
        {
            Empleado? _EmpPorIdEmpl = null;
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_ListarEmpleadoPorIdEmpleado", conn);
                cmd.Parameters.AddWithValue("@idEmpleado", IdEmpl);
                cmd.CommandType= CommandType.StoredProcedure;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        _EmpPorIdEmpl = new Empleado
                        {
                            IdEmpleado = Convert.ToInt32(dr["idEmpleado"]),
                            NombreEmpleado = dr["nombreEmpleado"].ToString() ?? "",
                            ReferenciaDepartamento = new Departamento
                            {
                                IdDepartamento = Convert.ToInt32(dr["idDepartamento"]),
                                NombreDepartamento = dr["nombreDepartamento"].ToString() ?? ""
                            },
                            Activo = Convert.ToBoolean(dr["activo"])
                        };
                    }
                }
            }
            return _EmpPorIdEmpl;
        }
        #endregion

        #region
        public async Task<List<Empleado>> ListarEmpPorNom(string nomEmp)
        {
            var _EmplPorNombre = new List<Empleado>();
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_ListarEmpleadoPornomEmpl", conn);
                cmd.Parameters.AddWithValue("@nomEmpl", nomEmp);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        _EmplPorNombre.Add(new Empleado
                        {
                            IdEmpleado = Convert.ToInt32(dr["idEmpleado"]),
                            NombreEmpleado = dr["nombreEmpleado"].ToString() ?? "",
                            ReferenciaDepartamento = new Departamento
                            {
                                IdDepartamento = Convert.ToInt32(dr["idDepartamento"]),
                                NombreDepartamento = dr["nombreDepartamento"].ToString() ?? "",
                            },
                            Activo = Convert.ToBoolean(dr["activo"])
                        });
                    }
                }
            }
            return _EmplPorNombre;
        }
        #endregion

        #region
        public async Task<bool> GuardarEmpleado(Empleado empleado)
        {
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_GuardarEmpleado", conn);
                cmd.Parameters.AddWithValue("@nombreEmpleado", empleado.NombreEmpleado);
                cmd.Parameters.AddWithValue("@idDepartamento",empleado.ReferenciaDepartamento.IdDepartamento);
                cmd.CommandType=CommandType.StoredProcedure;

                var _filasAfectadas = await cmd.ExecuteNonQueryAsync();
                if (_filasAfectadas>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        #endregion

        #region
        public async Task<bool> EditarEmpleado(Empleado empleado)
        {
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_EditarEmpleado", conn);
                cmd.Parameters.AddWithValue("@idEmpleado", empleado.IdEmpleado);
                cmd.Parameters.AddWithValue("@nombreEmpleado", empleado.NombreEmpleado);
                cmd.Parameters.AddWithValue("@idDepartamento", empleado.ReferenciaDepartamento.IdDepartamento);
                cmd.Parameters.AddWithValue("@activo", empleado.Activo);
                cmd.CommandType = CommandType.StoredProcedure;

                var _filasAfectadas = await cmd.ExecuteNonQueryAsync();
                if (_filasAfectadas > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion 

        #region
        public async Task<bool> EliminarEmplFisico(int idEmpl)
        {
            using (var conn  = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_EliminarEmpleadoFisico", conn);
                cmd.Parameters.AddWithValue("@idEmpleado",idEmpl);
                cmd.CommandType = CommandType.StoredProcedure;

                var _filasAfectadas = await cmd.ExecuteNonQueryAsync();
                if (_filasAfectadas > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region
        public async Task<bool> EstadoEmpl(int idEmpl)
        {
            using (var conn = new SqlConnection(_cadenaConn))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("sp_CambiarEstadoEmpl", conn);
                cmd.Parameters.AddWithValue("@idEmpleado",idEmpl);
                cmd.CommandType = CommandType.StoredProcedure;

                var _filasAfectadas = await cmd.ExecuteNonQueryAsync();
                if (_filasAfectadas > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

    }
}
